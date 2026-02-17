using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WeCare.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using WeCare.Shared;
using WeCare.Guests;
using WeCare.Responsibles;
using Volo.Abp.Identity;
using Volo.Abp.Guids;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.TenantManagement;

using IdentityUser = Volo.Abp.Identity.IdentityUser;
using IdentityRole = Volo.Abp.Identity.IdentityRole;

namespace WeCare.Patients;

[Authorize(WeCarePermissions.Patients.Default)]
public class PatientAppService : ApplicationService, IPatientAppService
{
    private readonly IRepository<Patient, Guid> _repository;
    private readonly IRepository<Responsible, Guid> _responsibleRepository;
    private readonly IRepository<Guest, Guid> _guestRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ITenantRepository _tenantRepository;

    public PatientAppService(
        IRepository<Patient, Guid> repository,
        IRepository<Responsible, Guid> responsibleRepository,
        IRepository<Guest, Guid> guestRepository,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IGuidGenerator guidGenerator,
        ITenantRepository tenantRepository)
    {
        _repository = repository;
        _responsibleRepository = responsibleRepository;
        _guestRepository = guestRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _guidGenerator = guidGenerator;
        _tenantRepository = tenantRepository;
    }

    public async Task<PatientDto> GetAsync(Guid id)
    {
        // Security Check
        if (CurrentUser.IsInRole("Responsible"))
        {
            var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
            var patient = await _repository.GetAsync(id);
            if (responsible == null || patient.PrincipalResponsibleId != responsible.Id)
            {
                throw new UserFriendlyException("Você não tem permissão para acessar este paciente.");
            }
        }
        else if (CurrentUser.IsInRole("Guest"))
        {
            var guest = await _guestRepository.FirstOrDefaultAsync(g => g.UserId == CurrentUser.Id);
            if (guest == null || guest.PatientId != id)
            {
                throw new UserFriendlyException("Você não tem permissão para acessar este paciente.");
            }
        }

        var p = await _repository.GetAsync(id);
        return ObjectMapper.Map<Patient, PatientDto>(p);
    }

    public async Task<PagedResultDto<PatientDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _repository.GetQueryableAsync();

        // Filtro de Segurança
        if (CurrentUser.IsInRole("Responsible"))
        {
            var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
            if (responsible != null)
            {
                queryable = queryable.Where(x => x.PrincipalResponsibleId == responsible.Id);
            }
            else
            {
                queryable = queryable.Where(x => false);
            }
        }
        else if (CurrentUser.IsInRole("Guest"))
        {
            var guest = await _guestRepository.FirstOrDefaultAsync(g => g.UserId == CurrentUser.Id);
            if (guest != null)
            {
                queryable = queryable.Where(x => x.Id == guest.PatientId);
            }
            else
            {
                queryable = queryable.Where(x => false);
            }
        }

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var query = queryable
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? "Name" : input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var patients = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<PatientDto>(
            totalCount,
            ObjectMapper.Map<List<Patient>, List<PatientDto>>(patients)
        );
    }

    [Authorize(WeCarePermissions.Patients.Create)]
    public async Task<PatientDto> CreateAsync(CreateUpdatePatientDto input)
    {
        // Check Clinic Resource Limits
        if (CurrentTenant.Id.HasValue)
        {
            var tenant = await _tenantRepository.FindAsync(CurrentTenant.Id.Value);
            if (tenant != null)
            {
                var maxPatients = tenant.GetProperty<int?>("MaxPatients");
                if (maxPatients.HasValue)
                {
                    var currentCount = await _repository.CountAsync();
                    if (currentCount >= maxPatients.Value)
                    {
                         throw new UserFriendlyException($"O limite de {maxPatients.Value} pacientes para esta clínica foi atingido. Entre em contato com o administrador.");
                    }
                }
            }
        }

        var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.CPF == input.CPF);

        if (responsible == null)
        {
            responsible = new Responsible
            {
                NameResponsible = input.NameResponsible,
                EmailAddress = input.EmailAddress,
                CPF = input.CPF,
                PhoneNumber = input.PhoneNumber
            };

            await _responsibleRepository.InsertAsync(responsible);

            // Criar IdentityUser para o Responsável
            var existingUser = await _userManager.FindByEmailAsync(input.EmailAddress);
            if (existingUser != null)
            {
                 throw new UserFriendlyException("Este e-mail já está em uso por outro usuário.");
            }

            var user = new IdentityUser(
                _guidGenerator.Create(),
                input.ResponsibleUserName,
                input.EmailAddress,
                CurrentTenant.Id
            )
            {
                Name = input.NameResponsible
            };

            (await _userManager.CreateAsync(user, input.ResponsiblePassword)).CheckErrors();

            // Atribuir Role "Responsible" (Garante consistência com o seeder)
            const string roleName = "Responsible";
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(_guidGenerator.Create(), roleName, CurrentTenant.Id));
            }

            (await _userManager.AddToRoleAsync(user, roleName)).CheckErrors();

            // Vincular o UserId ao Responsible
            responsible.UserId = user.Id;
            await _responsibleRepository.UpdateAsync(responsible);
        }
        else
        {
            responsible.NameResponsible = input.NameResponsible;
            responsible.EmailAddress = input.EmailAddress;
            responsible.PhoneNumber = input.PhoneNumber;

            await _responsibleRepository.UpdateAsync(responsible);
        }

        var patient = ObjectMapper.Map<CreateUpdatePatientDto, Patient>(input);
        patient.PrincipalResponsibleId = responsible.Id;

        responsible.Patients.Add(patient);

        await _repository.InsertAsync(patient);

        await _responsibleRepository.UpdateAsync(responsible);

        return ObjectMapper.Map<Patient, PatientDto>(patient);
    }

    public async Task<PatientDto> UpdateAsync(Guid id, CreateUpdatePatientDto input)
    {
        var patient = await _repository.GetAsync(id);
        ObjectMapper.Map(input, patient);
        await _repository.UpdateAsync(patient);
        return ObjectMapper.Map<Patient, PatientDto>(patient);
    }

    [Authorize(WeCarePermissions.Patients.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    public async Task<ListResultDto<LookupDto<Guid>>> GetPatientLookupAsync()
    {
        var queryable = await _repository.GetQueryableAsync();

        if (CurrentUser.IsInRole("Responsible"))
        {
            var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
            if (responsible != null)
            {
                queryable = queryable.Where(x => x.PrincipalResponsibleId == responsible.Id);
            }
            else
            {
                queryable = queryable.Where(x => false);
            }
        }
        else if (CurrentUser.IsInRole("Guest"))
        {
            var guest = await _guestRepository.FirstOrDefaultAsync(g => g.UserId == CurrentUser.Id);
            if (guest != null)
            {
                queryable = queryable.Where(x => x.Id == guest.PatientId);
            }
            else
            {
                queryable = queryable.Where(x => false);
            }
        }

        var patients = await AsyncExecuter.ToListAsync(queryable);

        var lookupDtos = patients.Select(p => new LookupDto<Guid>
        {
            Id = p.Id,
            DisplayName = p.Name
        }).ToList();

        return new ListResultDto<LookupDto<Guid>>(lookupDtos);
    }
}
