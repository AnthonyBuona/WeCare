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
using WeCare.Permissions;
using WeCare.Patients;
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

        #region Métodos Originais Mantidos
        public override async Task<ConsultationDto> GetAsync(Guid id)
        {
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);
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
                return query.OrderBy(t => t.DateTime);
            }
            var sorting = input.Sorting
                .Replace("patientName", "Patient.Name", StringComparison.OrdinalIgnoreCase)
                .Replace("therapistName", "Therapist.Name", StringComparison.OrdinalIgnoreCase);
            return query.OrderBy(sorting);
        }
        #endregion

        #region Novo Método Corrigido
        public async Task<List<ObjectiveGroupDto>> GetGroupedByPatientAsync(Guid patientId)
        {
            // 1. Construir a consulta IQueryable
            var queryable = (await Repository.WithDetailsAsync(x => x.Therapist))
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.DateTime);

            // 2. Executar a consulta usando o AsyncExecuter (a forma correta na camada de aplicação)
            var consultations = await AsyncExecuter.ToListAsync(queryable);

            if (!consultations.Any())
            {
                return new List<ObjectiveGroupDto>();
            }

            // 3. Agrupar os resultados
            var grouped = consultations.GroupBy(c => c.Description);

            // 4. Mapear para o DTO de retorno
            var result = grouped.Select(group => new ObjectiveGroupDto
            {
                ObjectiveName = group.Key,
                Consultations = ObjectMapper.Map<List<Consultation>, List<ConsultationInGroupDto>>(group.ToList())
            }).ToList();

            return result;
        }
        #endregion
        [Authorize(WeCarePermissions.Consultations.Create)]
        public async Task CreateObjectiveAsync(CreateUpdateObjectiveDto input)
        {
            // Criamos uma nova entidade de consulta que representa o início do objetivo
            var consultation = new Consultation
            {
                PatientId = input.PatientId,
                TherapistId = input.TherapistId,
                Description = input.ObjectiveName, // O nome do objetivo é a descrição da primeira consulta
                DateTime = input.FirstConsultationDateTime,
                Specialty = input.Specialty
            };

            await Repository.InsertAsync(consultation);
        }
    }
}