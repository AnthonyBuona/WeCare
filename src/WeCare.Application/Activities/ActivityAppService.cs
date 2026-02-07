using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Shared;
using WeCare.Trainings;

namespace WeCare.Activities
{
    public class ActivityAppService : CrudAppService<
        Activity, ActivityDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateActivityDto>,
        IActivityAppService
    {
        private readonly IRepository<Training, Guid> _trainingRepository;

        public ActivityAppService(
            IRepository<Activity, Guid> repository,
            IRepository<Training, Guid> trainingRepository) : base(repository)
        {
            _trainingRepository = trainingRepository;
        }

        public override async Task<ActivityDto> GetAsync(Guid id)
        {
            var query = await Repository.WithDetailsAsync(x => x.Training);
            var entity = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.Id == id);
            if (entity == null)
            {
                throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Activity), id);
            }

            return ObjectMapper.Map<Activity, ActivityDto>(entity);
        }

        public override async Task<PagedResultDto<ActivityDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var query = await Repository.WithDetailsAsync(x => x.Training);
            
            var totalCount = await AsyncExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncExecuter.ToListAsync(query);
            var entityDtos = ObjectMapper.Map<List<Activity>, List<ActivityDto>>(entities);

            return new PagedResultDto<ActivityDto>(
                totalCount,
                entityDtos
            );
        }

        public async Task<ListResultDto<LookupDto<Guid>>> GetTrainingLookupAsync()
        {
            var trainings = await _trainingRepository.GetListAsync();
            
            var lookupDtos = trainings
                .Select(t => new LookupDto<Guid>(t.Id, t.Name))
                .ToList();

            return new ListResultDto<LookupDto<Guid>>(lookupDtos);
        }
    }
}