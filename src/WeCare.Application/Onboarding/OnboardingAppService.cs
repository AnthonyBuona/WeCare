using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using WeCare.Clinics;
using WeCare.Patients;
using WeCare.Responsibles;
using WeCare.Therapists;

namespace WeCare.Onboarding
{
    [Authorize]
    public class OnboardingAppService : ApplicationService, IOnboardingAppService
    {
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Therapist, Guid> _therapistRepository;
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly IRepository<Clinic, Guid> _clinicRepository;
        private readonly IClinicAppService _clinicAppService;
        private readonly IdentityUserManager _userManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly ICurrentTenant _currentTenant;

        public OnboardingAppService(
            IRepository<Patient, Guid> patientRepository,
            IRepository<Therapist, Guid> therapistRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Clinic, Guid> clinicRepository,
            IClinicAppService clinicAppService,
            IdentityUserManager userManager,
            IGuidGenerator guidGenerator,
            ICurrentTenant currentTenant)
        {
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;
            _responsibleRepository = responsibleRepository;
            _clinicRepository = clinicRepository;
            _clinicAppService = clinicAppService;
            _userManager = userManager;
            _guidGenerator = guidGenerator;
            _currentTenant = currentTenant;
        }

        public async Task<OnboardingStatusDto> GetStatusAsync()
        {
            var status = new OnboardingStatusDto
            {
                NeedsOnboarding = false,
                ClinicName = "Minha Clínica",
                ContactEmail = "",
                PrimaryColor = "#4da391",
                SecondaryColor = "#8c7fcd"
            };

            if (_currentTenant.Id == null)
            {
                return status;
            }

            // Check if onboarding is needed: 0 therapists OR 0 patients in this tenant
            var therapistCount = await _therapistRepository.GetCountAsync();
            var patientCount = await _patientRepository.GetCountAsync();

            if (therapistCount == 0 || patientCount == 0)
            {
                status.NeedsOnboarding = true;
            }

            // Load current clinic settings
            var clinic = await _clinicRepository.FirstOrDefaultAsync(x => x.TenantId == _currentTenant.Id);
            if (clinic != null)
            {
                status.ClinicName = clinic.Name ?? "Minha Clínica";
                status.ContactEmail = clinic.Email ?? "";
                status.PrimaryColor = clinic.PrimaryColor ?? "#4da391";
                status.SecondaryColor = clinic.SecondaryColor ?? "#8c7fcd";
            }

            return status;
        }

        public async Task CompleteOnboardingAsync(CompleteOnboardingInputDto input)
        {
            if (_currentTenant.Id == null)
            {
                throw new UserFriendlyException("Essa funcionalidade só está disponível para clínicas configuradas (Multi-tenant).");
            }

            // 1. Update/Create Clinic settings
            var clinic = await _clinicRepository.FirstOrDefaultAsync(x => x.TenantId == _currentTenant.Id);
            if (clinic == null)
            {
                clinic = new Clinic
                {
                    TenantId = _currentTenant.Id,
                    Name = input.ClinicName,
                    Email = input.ContactEmail,
                    PrimaryColor = input.PrimaryColor,
                    SecondaryColor = input.SecondaryColor,
                    Status = ClinicStatus.Active
                };
                await _clinicRepository.InsertAsync(clinic, autoSave: true);
            }
            else
            {
                clinic.Name = input.ClinicName;
                clinic.Email = input.ContactEmail;
                clinic.PrimaryColor = input.PrimaryColor;
                clinic.SecondaryColor = input.SecondaryColor;
                await _clinicRepository.UpdateAsync(clinic, autoSave: true);
            }

            // 2. Create Therapist
            var therapistUser = await CreateIdentityUserAsync(input.TherapistEmail, "Clinic123!", "Therapist");
            var therapist = new Therapist
            {
                Name = input.TherapistName,
                Email = input.TherapistEmail,
                UserId = therapistUser.Id,
                Specialization = input.TherapistSpecialization ?? "Terapia ABA"
            };
            await _therapistRepository.InsertAsync(therapist, autoSave: true);

            // 3. Create Responsible
            var responsibleUser = await CreateIdentityUserAsync(input.ResponsibleEmail, "Parent123!", "Responsible");
            var responsible = new Responsible
            {
                NameResponsible = input.ResponsibleName,
                CPF = input.ResponsibleCpf,
                EmailAddress = input.ResponsibleEmail,
                UserId = responsibleUser.Id
            };
            await _responsibleRepository.InsertAsync(responsible, autoSave: true);

            // 4. Create Patient
            var patient = new Patient
            {
                Name = input.PatientName,
                BirthDate = input.PatientBirthDate,
                PrincipalResponsibleId = responsible.Id
            };
            await _patientRepository.InsertAsync(patient, autoSave: true);
        }

        private async Task<Volo.Abp.Identity.IdentityUser> CreateIdentityUserAsync(string email, string password, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new Volo.Abp.Identity.IdentityUser(_guidGenerator.Create(), email, email, _currentTenant.Id);
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new UserFriendlyException("Erro ao criar usuário de acesso: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
                var roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    throw new UserFriendlyException("Erro ao associar perfil de acesso: " + string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }
            }
            return user;
        }
    }
}
