using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using WeCare.Permissions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using System.Linq.Dynamic.Core;
using Volo.Abp;
using WeCare.Responsibles;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using WeCare.Shared;

namespace WeCare.Patients;

[Authorize(WeCarePermissions.Patients.Default)]
public class PatientAppService : ApplicationService, IPatientAppService
{
    private readonly IRepository<Patient, Guid> _repository;
    private readonly IRepository<Responsible, Guid> _responsibleRepository;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly IGuidGenerator _guidGenerator;

    public PatientAppService(
        IRepository<Patient, Guid> repository,
        IRepository<Responsible, Guid> responsibleRepository,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IGuidGenerator guidGenerator)
    {
        _repository = repository;
        _responsibleRepository = responsibleRepository;
        _userManager = userManager;
        _roleManager = roleManager;
        _guidGenerator = guidGenerator;
    }

    public async Task<PatientDto> GetAsync(Guid id)
    {
        var patient = await _repository.GetAsync(id);
        return ObjectMapper.Map<Patient, PatientDto>(patient);
    }

    public async Task<PagedResultDto<PatientDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        var query = queryable
            .OrderBy(input.Sorting.IsNullOrWhiteSpace() ? "Name" : input.Sorting)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var patients = await AsyncExecuter.ToListAsync(query);
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        return new PagedResultDto<PatientDto>(
            totalCount,
            ObjectMapper.Map<List<Patient>, List<PatientDto>>(patients)
        );
    }

    [Authorize(WeCarePermissions.Patients.Create)]
    public async Task<PatientDto> CreateAsync(CreateUpdatePatientDto input)
    {
        var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.CPF == input.CPF);

        if (responsible == null)
        {
            responsible = new Responsible
            {
                NameResponsible = input.NameResponsible,
                EmailAddress = input.EmailAddress,
                CPF = input.CPF
            };

            await _responsibleRepository.InsertAsync(responsible);

            var slug = input.NameResponsible
            .Trim()
            .ToLowerInvariant()
            .Replace(" ", ".");

            var user = new IdentityUser(
                _guidGenerator.Create(),
                slug,
                input.EmailAddress
            );

            var createResult = await _userManager.CreateAsync(user, "A1801b0811;");

            if (!createResult.Succeeded)
            {
                throw new UserFriendlyException("Erro ao criar o usuário responsável: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }

            if (!await _roleManager.RoleExistsAsync("Responsável"))
            {
                await _roleManager.CreateAsync(new IdentityRole(_guidGenerator.Create(), "Responsável", CurrentTenant.Id));
            }

            await _userManager.AddToRoleAsync(user, "Responsável");
        }
        else
        {
            responsible.NameResponsible = input.NameResponsible;
            responsible.EmailAddress = input.EmailAddress;

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
        var patients = await _repository.GetListAsync();

        var lookupDtos = patients.Select(p => new LookupDto<Guid>
        {
            Id = p.Id,
            DisplayName = p.Name
        }).ToList();

        return new ListResultDto<LookupDto<Guid>>(lookupDtos);
    }
}
