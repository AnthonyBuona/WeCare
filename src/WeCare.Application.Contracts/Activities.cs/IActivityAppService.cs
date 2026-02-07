using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Shared;
using System.Threading.Tasks;

namespace WeCare.Activities
{
    public interface IActivityAppService : ICrudAppService<
        ActivityDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateActivityDto>
    {
        Task<ListResultDto<LookupDto<Guid>>> GetTrainingLookupAsync();
    }
}