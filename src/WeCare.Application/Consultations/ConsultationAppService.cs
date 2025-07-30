using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Application.Contracts.Consultations;
using WeCare.Application.Contracts.PerformedTrainings; // IMPORTANTE: Adicionar este using
using WeCare.Permissions;
using WeCare.PerformedTrainings; // IMPORTANTE: Adicionar este using
using WeCare.Therapists;


namespace WeCare.Consultations
{
    [Authorize(WeCarePermissions.Consultations.Default)]
    public class ConsultationAppService : CrudAppService<
            Consultation,
            ConsultationDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdateConsultationDto>,
        IConsultationAppService
    {
        private readonly IRepository<Therapist, Guid> _therapistRepository;

        public ConsultationAppService(
            IRepository<Consultation, Guid> repository,
            IRepository<Therapist, Guid> therapistRepository)
            : base(repository)
        {
            _therapistRepository = therapistRepository;
            GetPolicyName = WeCarePermissions.Consultations.Default;
            GetListPolicyName = WeCarePermissions.Consultations.Default;
            CreatePolicyName = WeCarePermissions.Consultations.Create;
            UpdatePolicyName = WeCarePermissions.Consultations.Edit;
            DeletePolicyName = WeCarePermissions.Consultations.Delete;
        }

        // NOVO MÉTODO CreateAsync AQUI!
        [Authorize(WeCarePermissions.Consultations.Create)]
        public override async Task<ConsultationDto> CreateAsync(CreateUpdateConsultationDto input)
        {
            // 1. Mapeia os dados simples do DTO (MainTraining, Duration, etc.) para a entidade Consulta.
            var consultation = ObjectMapper.Map<CreateUpdateConsultationDto, Consultation>(input);

            // 2. Garante que a coleção de treinos não seja nula.
            consultation.PerformedTrainings = new List<PerformedTraining>();

            // 3. Itera sobre cada DTO de treino que veio do frontend.
            foreach (var trainingDto in input.PerformedTrainings)
            {
                // Mapeia o DTO de treino para a entidade PerformedTraining.
                var training = ObjectMapper.Map<CreateUpdatePerformedTrainingDto, PerformedTraining>(trainingDto);
                // Adiciona o treino na coleção da consulta.
                consultation.PerformedTrainings.Add(training);
            }

            // 4. Insere a consulta no banco de dados.
            // O Entity Framework salvará a consulta e todos os seus treinos associados de uma só vez.
            await Repository.InsertAsync(consultation, autoSave: true);

            // 5. Retorna o DTO da consulta que acabou de ser criada.
            return ObjectMapper.Map<Consultation, ConsultationDto>(consultation);
        }

        #region Métodos Originais Mantidos
        public override async Task<ConsultationDto> GetAsync(Guid id)
        {
            // Para carregar os treinos junto com a consulta, adicione .WithDetailsAsync() aqui também
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist, x => x.PerformedTrainings);
            var query = queryable.Where(x => x.Id == id);
            var consultation = await AsyncExecuter.FirstOrDefaultAsync(query);
            return ObjectMapper.Map<Consultation, ConsultationDto>(consultation);
        }

        public override async Task<PagedResultDto<ConsultationDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);
            queryable = ApplySorting(queryable, input);
            queryable = ApplyPaging(queryable, input);
            var consultations = await AsyncExecuter.ToListAsync(queryable);
            var totalCount = await Repository.GetCountAsync();
            return new PagedResultDto<ConsultationDto>(
                totalCount,
                ObjectMapper.Map<List<Consultation>, List<ConsultationDto>>(consultations)
            );
        }

        protected override IQueryable<Consultation> ApplySorting(IQueryable<Consultation> query, PagedAndSortedResultRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                // A propriedade DateTime foi movida para ConsultationDate no ViewModel,
                // mas a entidade ainda pode ter DateTime. Verifique o nome correto.
                // Se a entidade foi atualizada para ConsultationDate, troque aqui.
                return query.OrderBy(t => t.DateTime);
            }
            var sorting = input.Sorting
                .Replace("patientName", "Patient.Name", StringComparison.OrdinalIgnoreCase)
                .Replace("therapistName", "Therapist.Name", StringComparison.OrdinalIgnoreCase);
            return query.OrderBy(sorting);
        }
        #endregion

        #region Métodos de Objetivo (antigos)
        public async Task<List<ObjectiveGroupDto>> GetGroupedByPatientAsync(Guid patientId)
        {
            var queryable = (await Repository.WithDetailsAsync(x => x.Therapist))
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.DateTime);

            var consultations = await AsyncExecuter.ToListAsync(queryable);

            if (!consultations.Any())
            {
                return new List<ObjectiveGroupDto>();
            }

            var grouped = consultations.GroupBy(c => c.Description);

            var result = grouped.Select(group => new ObjectiveGroupDto
            {
                ObjectiveName = group.Key,
                Consultations = ObjectMapper.Map<List<Consultation>, List<ConsultationInGroupDto>>(group.ToList())
            }).ToList();

            return result;
        }

        [Authorize(WeCarePermissions.Consultations.Create)]
        public async Task CreateObjectiveAsync(CreateUpdateObjectiveDto input)
        {
            var consultation = new Consultation
            {
                PatientId = input.PatientId,
                TherapistId = input.TherapistId,
                Description = input.ObjectiveName,
                DateTime = input.FirstConsultationDateTime,
                Specialty = input.Specialty
            };

            await Repository.InsertAsync(consultation);
        }

        public async Task<ListResultDto<string>> GetObjectiveNamesForPatientAsync(Guid patientId)
        {
            var queryable = await Repository.GetQueryableAsync();
            var query = queryable
                .Where(c => c.PatientId == patientId)
                .Select(c => c.Description)
                .Distinct();

            var objectiveNames = await AsyncExecuter.ToListAsync(query);

            return new ListResultDto<string>(objectiveNames);
        }
        #endregion
    }
}