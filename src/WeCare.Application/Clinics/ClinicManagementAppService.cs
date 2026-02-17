using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.TenantManagement;
using WeCare.Permissions;
using System.Linq;

namespace WeCare.Clinics;

[Authorize(WeCarePermissions.Clinics.Default)]
public class ClinicManagementAppService : WeCareAppService, IClinicManagementAppService
{
    private readonly ITenantAppService _tenantAppService;
    private readonly IRepository<Clinic, Guid> _clinicRepository;
    private readonly IDataFilter _dataFilter;

    public ClinicManagementAppService(
        ITenantAppService tenantAppService,
        IRepository<Clinic, Guid> clinicRepository,
        IDataFilter dataFilter)
    {
        _tenantAppService = tenantAppService;
        _clinicRepository = clinicRepository;
        _dataFilter = dataFilter;
    }

    public async Task<ClinicDto> CreateAsync(CreateClinicInput input)
    {
        if (CurrentTenant.Id != null)
        {
            throw new Volo.Abp.UserFriendlyException("Only the host can create new clinics.");
        }

        // 1. Criar o Tenant usando o serviço do módulo TenantManagement
        var tenantCreateInput = new TenantCreateDto
        {
            Name = input.Name,
            AdminEmailAddress = input.AdminEmailAddress,
            AdminPassword = input.AdminPassword
        };

        var tenantDto = await _tenantAppService.CreateAsync(tenantCreateInput);

        // Atualizar o tenant com as credenciais administrativas nas ExtraProperties
        var tenant = await _tenantAppService.GetAsync(tenantDto.Id);
        tenant.SetProperty("AdminEmail", input.AdminEmailAddress);
        tenant.SetProperty("AdminPassword", input.AdminPassword);
        await _tenantAppService.UpdateAsync(tenant.Id, ObjectMapper.Map<TenantDto, TenantUpdateDto>(tenant));

        // 2. Criar a entidade Clinic vinculada a esse Tenant
        var clinic = new Clinic
        {
            TenantId = tenantDto.Id,
            Name = input.Name,
            CNPJ = input.CNPJ,
            Address = input.Address,
            Phone = input.Phone,
            Email = input.AdminEmailAddress, // Usando o email do admin como contato principal inicialmente
            Specializations = input.Specializations,
            Status = ClinicStatus.Active
        };

        // Necessário desabilitar o filtro IMultiTenant para salvar uma entidade com TenantId diferente do atual (null/Host)
        using (_dataFilter.Disable<IMultiTenant>())
        {
            await _clinicRepository.InsertAsync(clinic);
        }

        return ObjectMapper.Map<Clinic, ClinicDto>(clinic);
    }

    public async Task<PagedResultDto<ClinicDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        // Listar todas as clínicas (TenantId != null)
        using (_dataFilter.Disable<IMultiTenant>())
        {
            var query = await _clinicRepository.GetQueryableAsync();
            
            // Aqui você pode adicionar filtros de busca se necessário
            
            var totalCount = await AsyncExecuter.CountAsync(query);
            
            var clinics = await AsyncExecuter.ToListAsync(query.Skip(input.SkipCount).Take(input.MaxResultCount));

            return new PagedResultDto<ClinicDto>(
                totalCount,
                ObjectMapper.Map<List<Clinic>, List<ClinicDto>>(clinics)
            );
        }
    }
}
