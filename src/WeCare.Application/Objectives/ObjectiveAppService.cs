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
using WeCare.Patients;
using WeCare.Permissions;
using WeCare.Therapists;
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
        private readonly IRepository<Training, Guid> _trainingRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Therapist, Guid> _therapistRepository;

        public ObjectiveAppService(
            IRepository<Objective, Guid> repository,
            IRepository<Training, Guid> trainingRepository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Therapist, Guid> therapistRepository) : base(repository)
        {
            _trainingRepository = trainingRepository;
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;

            GetPolicyName = WeCarePermissions.Objectives.Default;
            GetListPolicyName = WeCarePermissions.Objectives.Default;
            CreatePolicyName = WeCarePermissions.Objectives.Create;
            UpdatePolicyName = WeCarePermissions.Objectives.Edit;
            DeletePolicyName = WeCarePermissions.Objectives.Delete;
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

        public async Task<ListResultDto<LookupDto<Guid>>> GetPatientLookupAsync()
        {
            var patients = await _patientRepository.GetListAsync();
            return new ListResultDto<LookupDto<Guid>>(
                ObjectMapper.Map<List<Patient>, List<LookupDto<Guid>>>(patients)
            );
        }

        public async Task<ListResultDto<LookupDto<Guid>>> GetTherapistLookupAsync()
        {
            var therapists = await _therapistRepository.GetListAsync();
            return new ListResultDto<LookupDto<Guid>>(
                ObjectMapper.Map<List<Therapist>, List<LookupDto<Guid>>>(therapists)
            );
        }
    }
}