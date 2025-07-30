using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace WeCare.Activities
{
    public interface IActivityAppService : ICrudAppService<
        ActivityDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateActivityDto>
    {
    }
}