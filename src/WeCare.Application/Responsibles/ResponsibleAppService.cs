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

namespace WeCare.Responsibles;

[Authorize(WeCarePermissions.Responsibles.Default)]
public class ResponsibleAppService : ApplicationService, IResponsibleAppService
{
    private readonly IRepository<Responsible, Guid> _repository;

    public ResponsibleAppService(IRepository<Responsible, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<ResponsibleDto> GetAsync(Guid id)
    {
        var Responsible = await _repository.GetAsync(id);
        return ObjectMapper.Map<Responsible, ResponsibleDto>(Responsible);
    }

    public async Task<PagedResultDto<ResponsibleDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _repository.GetQueryableAsync();
        var query = queryable
            .OrderBy(p=> p.NameResponsible)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var Responsibles = await AsyncExecuter.ToListAsync(query);
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        return new PagedResultDto<ResponsibleDto>(
            totalCount,
            ObjectMapper.Map<List<Responsible>, List<ResponsibleDto>>(Responsibles)
        );
    }

    [Authorize(WeCarePermissions.Responsibles.Create)]
    public async Task<ResponsibleDto> CreateAsync(CreateUpdateResponsibleDto input)
    {
        var Responsible = ObjectMapper.Map<CreateUpdateResponsibleDto, Responsible>(input);
        await _repository.InsertAsync(Responsible);
        return ObjectMapper.Map<Responsible, ResponsibleDto>(Responsible);
    }

    //[Authorize(WeCarePermissions.Responsibles.Edit)]
    public async Task<ResponsibleDto> UpdateAsync(Guid id, CreateUpdateResponsibleDto input)
    {
        var Responsible = await _repository.GetAsync(id);
        ObjectMapper.Map(input, Responsible); // Aplica as alterações
        await _repository.UpdateAsync(Responsible);
        return ObjectMapper.Map<Responsible, ResponsibleDto>(Responsible);
    }

    [Authorize(WeCarePermissions.Responsibles.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}
