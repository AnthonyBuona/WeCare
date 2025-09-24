using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Activities;

namespace WeCare.Trainings
{
    public class TrainingAppService : CrudAppService<
        Training, TrainingDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateTrainingDto>,
        ITrainingAppService
    {
        private readonly IRepository<Activity, Guid> _activityRepository;

        public TrainingAppService(
            IRepository<Training, Guid> repository,
            IRepository<Activity, Guid> activityRepository) : base(repository)
        {
            _activityRepository = activityRepository;
        }

        //// Sobrescrevemos para incluir as atividades ao buscar um único treino
        //public override async Task<TrainingDto> GetAsync(Guid id)
        //{
        //    var queryable = await Repository.WithDetailsAsync(t => t.Activities);
        //    var training = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(t => t.Id == id));
        //    return ObjectMapper.Map<Training, TrainingDto>(training);
        //}

        //// Sobrescrevemos para incluir as atividades na listagem
        //public override async Task<PagedResultDto<TrainingDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        //{
        //    var queryable = await Repository.WithDetailsAsync(t => t.Activities);
        //    var totalCount = await AsyncExecuter.CountAsync(queryable);
        //    var trainings = await AsyncExecuter.ToListAsync(
        //        ApplyPaging(ApplySorting(queryable, input), input)
        //    );
        //    return new PagedResultDto<TrainingDto>(
        //        totalCount,
        //        ObjectMapper.Map<List<Training>, List<TrainingDto>>(trainings)
        //    );
        //}

        //// Implementação do método extra
        public async Task<ListResultDto<ActivityDto>> GetActivitiesAsync(Guid trainingId)
        {
            var activities = await _activityRepository.GetListAsync(a => a.TrainingId == trainingId);
            return new ListResultDto<ActivityDto>(
                ObjectMapper.Map<List<Activity>, List<ActivityDto>>(activities)
            );
        }
    }
}