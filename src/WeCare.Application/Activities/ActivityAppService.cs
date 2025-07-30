using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace WeCare.Activities
{
    public class ActivityAppService : CrudAppService<
        Activity, ActivityDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateActivityDto>,
        IActivityAppService
    {
        public ActivityAppService(IRepository<Activity, Guid> repository) : base(repository)
        {
        }
    }
}