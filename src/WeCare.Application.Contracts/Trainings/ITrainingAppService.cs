using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using WeCare.Activities;

namespace WeCare.Trainings
{
    public interface ITrainingAppService : ICrudAppService<
        TrainingDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateTrainingDto>
    {
        Task<ListResultDto<ActivityDto>> GetActivitiesAsync(Guid trainingId);
    }
}