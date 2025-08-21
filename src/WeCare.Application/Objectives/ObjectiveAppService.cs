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
        public ObjectiveAppService(IRepository<Objective, Guid> repository) : base(repository)
        {
            // Aqui você pode definir as permissões se necessário
        }

        public override async Task<ObjectiveDto> CreateAsync(CreateUpdateObjectiveDto input)
        {
            // 1. Mapeia o DTO para a entidade Objetivo.
            var objective = ObjectMapper.Map<CreateUpdateObjectiveDto, Objective>(input);

            // 2. Insere APENAS o objetivo no banco de dados.
            await Repository.InsertAsync(objective, autoSave: true);

            // 3. Retorna o DTO do objetivo que acabou de ser criado.
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
    }
}