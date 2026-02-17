using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Uow;
using WeCare.Patients;
using WeCare.Responsibles;
using WeCare.Permissions;
using WeCare.Shared;
using Volo.Abp.Data;
using Volo.Abp.TenantManagement;

// Alias to avoid ambiguity
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace WeCare.Guests
{
    public class GuestAppService : CrudAppService<
            Guest,
            GuestDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateGuestDto>,
        IGuestAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly ITenantRepository _tenantRepository;

        public GuestAppService(
            IRepository<Guest, Guid> repository,
            IdentityUserManager userManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            ITenantRepository tenantRepository)
            : base(repository)
        {
            _userManager = userManager;
            _unitOfWorkManager = unitOfWorkManager;
            _patientRepository = patientRepository;
            _responsibleRepository = responsibleRepository;
            _tenantRepository = tenantRepository;

            GetPolicyName = WeCarePermissions.Guests.Default;
            GetListPolicyName = WeCarePermissions.Guests.Default;
            CreatePolicyName = WeCarePermissions.Guests.Create;
            UpdatePolicyName = WeCarePermissions.Guests.Edit;
            DeletePolicyName = WeCarePermissions.Guests.Delete;
        }

        // Override Create to handle IdentityUser creation
        [Authorize(WeCarePermissions.Guests.Create)]
        public override async Task<GuestDto> CreateAsync(CreateUpdateGuestDto input)
        {
            // Check Clinic Resource Limits (Max Guests Per Patient)
            if (CurrentTenant.Id.HasValue)
            {
                var tenant = await _tenantRepository.FindAsync(CurrentTenant.Id.Value);
                if (tenant != null)
                {
                    var maxGuests = tenant.GetProperty<int?>("MaxGuestsPerPatient");
                    if (maxGuests.HasValue)
                    {
                        var currentCount = await Repository.CountAsync(g => g.PatientId == input.PatientId);
                        if (currentCount >= maxGuests.Value)
                        {
                            throw new UserFriendlyException($"O limite de {maxGuests.Value} convidados por paciente para esta clínica foi atingido. Entre em contato com o administrador.");
                        }
                    }
                }
            }

            // Restrição: Se o usuário logado for um "Responsável", ele só pode criar convidados para o SEU paciente.
            if (CurrentUser.IsInRole("Responsible"))
            {
                var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
                if (responsible == null)
                {
                    throw new UserFriendlyException("Seu perfil de responsável não foi encontrado.");
                }

                var patient = await _patientRepository.GetAsync(input.PatientId);
                if (patient.PrincipalResponsibleId != responsible.Id)
                {
                    throw new UserFriendlyException("Você só pode criar convidados para o paciente sob sua responsabilidade.");
                }

                // Forçar o ResponsibleId do convidado para ser o do usuário logado
                input.ResponsibleId = responsible.Id;
            }

            // Gerar UserName automaticamente a partir do nome
            if (string.IsNullOrWhiteSpace(input.UserName))
            {
                input.UserName = input.Name.Replace(" ", "");
            }

            var existingUser = await _userManager.FindByNameAsync(input.UserName);
            if (existingUser != null)
            {
                // Se já existir, adicionar um sufixo aleatório ou incremental (opcional, mas seguro)
                input.UserName += Guid.NewGuid().ToString().Substring(0, 4);
            }

            // 1. Create IdentityUser
            var existingEmailUser = await _userManager.FindByEmailAsync(input.Email);
            if (existingEmailUser != null)
            {
                 throw new UserFriendlyException("Este e-mail já está em uso por outro usuário.");
            }

            var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.Email)
            {
                Name = input.Name
            };
            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();

            // 2. Assign "Guest" Role
            const string guestRole = "Guest"; 
            // Note: Ensure "Guest" role exists in DB (WeCareRoleDataSeeder)
            (await _userManager.AddToRoleAsync(user, guestRole)).CheckErrors();

            // 3. Save to ensure User is persisted before Guest (though UoW handles transaction)
            if (_unitOfWorkManager.Current != null)
            {
                await _unitOfWorkManager.Current.SaveChangesAsync();
            }

            // 4. Create Guest entity linked to User
            var guest = new Guest
            {
                Name = input.Name,
                Email = input.Email,
                Relationship = input.Relationship,
                PatientId = input.PatientId,
                ResponsibleId = input.ResponsibleId,
                // Wait, if ResponsibleId is nullable in entity, we can pass null?
                // But if the entity has ResponsibleId as Guid? (nullable), it's fine.
                // However, check if we need to set TenantId? CrudAppService handles it if entity implements IMultiTenant.
                UserId = user.Id
            };

            await Repository.InsertAsync(guest, autoSave: true);

            return ObjectMapper.Map<Guest, GuestDto>(guest);
        }

        public async Task<ListResultDto<LookupDto<Guid>>> GetPatientLookupAsync()
        {
            var patients = await _patientRepository.GetListAsync();
            return new ListResultDto<LookupDto<Guid>>(
                ObjectMapper.Map<List<Patient>, List<LookupDto<Guid>>>(patients)
            );
        }

        public async Task<ListResultDto<LookupDto<Guid>>> GetResponsibleLookupAsync()
        {
            var responsibles = await _responsibleRepository.GetListAsync();
            return new ListResultDto<LookupDto<Guid>>(
                responsibles.Select(r => new LookupDto<Guid>(r.Id, r.NameResponsible)).ToList()
            );
        }

        // Override GetList to include Patient Name manually
        public override async Task<PagedResultDto<GuestDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            // 1. Get the list of guests with paging/sorting
            var queryable = await Repository.GetQueryableAsync();
            var totalCount = await AsyncExecuter.CountAsync(queryable);
            
            queryable = ApplySorting(queryable, input);
            queryable = ApplyPaging(queryable, input);

            var guests = await AsyncExecuter.ToListAsync(queryable);

            // 2. Map to DTOs
            var guestDtos = ObjectMapper.Map<List<Guest>, List<GuestDto>>(guests);

            // 3. Fetch Patient Names manually to avoid EF Core reference issues in App Layer
            if (guestDtos.Any())
            {
                var patientIds = guestDtos.Select(g => g.PatientId).Distinct().ToList();
                var patients = await _patientRepository.GetListAsync(p => patientIds.Contains(p.Id));
                var patientDict = patients.ToDictionary(p => p.Id, p => p.Name);

                foreach (var dto in guestDtos)
                {
                    if (patientDict.TryGetValue(dto.PatientId, out var patientName))
                    {
                        dto.PatientName = patientName;
                    }
                }
            }

            return new PagedResultDto<GuestDto>(
                totalCount,
                guestDtos
            );
        }
    }
}
