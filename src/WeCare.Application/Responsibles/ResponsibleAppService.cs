using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WeCare.Permissions;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using System.Linq.Dynamic.Core;

namespace WeCare.Responsibles;

[Authorize(WeCarePermissions.Responsibles.Default)]
public class ResponsibleAppService : ApplicationService, IResponsibleAppService
{
    private readonly IRepository<Responsible, Guid> _repository;
    private readonly IdentityUserManager _userManager;

    public ResponsibleAppService(
        IRepository<Responsible, Guid> repository,
        IdentityUserManager userManager)
    {
        _repository = repository;
        _userManager = userManager;
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
        // 1. Criar IdentityUser
        var user = new Volo.Abp.Identity.IdentityUser(
            GuidGenerator.Create(),
            input.UserName,
            input.EmailAddress,
            CurrentTenant.Id
        )
        {
            Name = input.NameResponsible
        };

        (await _userManager.CreateAsync(user, input.Password)).CheckErrors();
        
        // 2. Atribuir Role "Responsible"
        (await _userManager.AddToRoleAsync(user, "Responsible")).CheckErrors();

        // 3. Criar entidade Responsible vinculada
        var responsible = ObjectMapper.Map<CreateUpdateResponsibleDto, Responsible>(input);
        responsible.UserId = user.Id;

        await _repository.InsertAsync(responsible);
        
        return ObjectMapper.Map<Responsible, ResponsibleDto>(responsible);
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
