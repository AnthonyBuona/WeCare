// Local: WeCare.Application/Objectives/ObjectiveAppService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Application.Contracts.Consultations;
using WeCare.Consultations;
using WeCare.Shared;
using WeCare.Trainings;

namespace WeCare.Objectives
{
    public class ObjectiveAppService : CrudAppService<
        Objective,
        ObjectiveDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateUpdateObjectiveDto>,
        IObjectiveAppService
    {
        // 1. Declare o repositório de treinos que será usado.
        private readonly IRepository<Training, Guid> _trainingRepository;

        // 2. Modifique o construtor para receber o IRepository<Training, Guid>.
        public ObjectiveAppService(
            IRepository<Objective, Guid> repository,
            IRepository<Training, Guid> trainingRepository) : base(repository) // Adicione trainingRepository aqui
        {
            // 3. Atribua o repositório injetado à sua variável local.
            _trainingRepository = trainingRepository;
        }

        public override async Task<ObjectiveDto> CreateAsync(CreateUpdateObjectiveDto input)
        {
            var objective = ObjectMapper.Map<CreateUpdateObjectiveDto, Objective>(input);
            await Repository.InsertAsync(objective, autoSave: true);
            return ObjectMapper.Map<Objective, ObjectiveDto>(objective);
        }

        public async Task<ListResultDto<LookupDto<Guid>>> GetObjectiveLookupAsync(Guid patientId)
        {
            var objectives = await Repository.GetListAsync(x => x.PatientId == patientId);
            var lookupDtos = objectives
                .Select(x => new LookupDto<Guid>(x.Id, x.Name))
                .ToList();
            return new ListResultDto<LookupDto<Guid>>(lookupDtos);
        }

        public async Task<List<ObjectiveGroupDto>> GetGroupedObjectivesByPatientAsync(Guid patientId)
        {
            var objectivesWithConsultations = await Repository.WithDetailsAsync(o => o.Consultations);
            var patientObjectives = objectivesWithConsultations
                .Where(o => o.PatientId == patientId)
                .OrderByDescending(o => o.StartDate)
                .ToList();

            if (!patientObjectives.Any())
            {
                return new List<ObjectiveGroupDto>();
            }

            var result = patientObjectives.Select(objective => new ObjectiveGroupDto
            {
                ObjectiveName = objective.Name,
                Consultations = ObjectMapper.Map<List<Consultation>, List<ConsultationInGroupDto>>(objective.Consultations.ToList())
            }).ToList();

            return result;
        }

        // Agora este método funcionará, pois _trainingRepository existe!
        public async Task<ListResultDto<LookupDto<Guid>>> GetTrainingsForObjective(Guid objectiveId)
        {
            var trainings = await _trainingRepository.GetListAsync(t => t.ObjectiveId == objectiveId);

            // O mapeamento aqui também precisa ser ajustado para funcionar com LookupDto
            var lookupDtos = trainings
                .Select(t => new LookupDto<Guid>(t.Id, t.Name)) // Supondo que Training tenha uma propriedade "Name"
                .ToList();

            return new ListResultDto<LookupDto<Guid>>(lookupDtos);
        }
    }
}