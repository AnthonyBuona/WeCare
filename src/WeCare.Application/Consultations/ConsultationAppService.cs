using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Application.Contracts.Consultations;
using WeCare.Application.Contracts.PerformedTrainings;
using WeCare.Clinics;
using WeCare.Guests;
using WeCare.PerformedTrainings;
using WeCare.Permissions;
using WeCare.Responsibles;
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
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly IRepository<Guest, Guid> _guestRepository;
        private readonly IClinicAppService _clinicAppService;

        public ConsultationAppService(
            IRepository<Consultation, Guid> repository,
            IRepository<Therapist, Guid> therapistRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Guest, Guid> guestRepository,
            IClinicAppService clinicAppService)
            : base(repository)
        {
            _therapistRepository = therapistRepository;
            _responsibleRepository = responsibleRepository;
            _guestRepository = guestRepository;
            _clinicAppService = clinicAppService;

            GetPolicyName = WeCarePermissions.Consultations.Default;
            GetListPolicyName = WeCarePermissions.Consultations.Default;
            CreatePolicyName = WeCarePermissions.Consultations.Create;
            UpdatePolicyName = WeCarePermissions.Consultations.Edit;
            DeletePolicyName = WeCarePermissions.Consultations.Delete;
        }

        [Authorize(WeCarePermissions.Consultations.Create)]
        public override async Task<ConsultationDto> CreateAsync(CreateUpdateConsultationDto input)
        {
            // 1. Validação de conflitos de horário
            await ValidateNoConflictAsync(input.PatientId, input.TherapistId, input.DateTime);

            // 2. Auto-preencher especialidade do terapeuta se não informada
            if (string.IsNullOrWhiteSpace(input.Specialty))
            {
                var therapist = await _therapistRepository.GetAsync(input.TherapistId);
                input.Specialty = therapist.Specialization ?? "Geral";
            }

            // 3. Mapeia os dados do DTO para a entidade Consulta.
            var consultation = ObjectMapper.Map<CreateUpdateConsultationDto, Consultation>(input);
            consultation.PerformedTrainings = new List<PerformedTraining>();

            foreach (var trainingDto in input.PerformedTrainings)
            {
                var training = ObjectMapper.Map<CreateUpdatePerformedTrainingDto, PerformedTraining>(trainingDto);
                consultation.PerformedTrainings.Add(training);
            }

            // 4. Insere a consulta no banco de dados.
            await Repository.InsertAsync(consultation, autoSave: true);

            return ObjectMapper.Map<Consultation, ConsultationDto>(consultation);
        }

        /// <summary>
        /// Valida se não existe conflito de horário para o paciente ou terapeuta.
        /// </summary>
        private async Task ValidateNoConflictAsync(Guid patientId, Guid therapistId, DateTime dateTime)
        {
            // Buscar duração do agendamento nas configurações da clínica (padrão 30min)
            var durationMinutes = 30;
            try
            {
                var settings = await _clinicAppService.GetCurrentClinicSettingsAsync();
                if (settings?.AppointmentDurationMinutes > 0)
                {
                    durationMinutes = settings.AppointmentDurationMinutes;
                }
            }
            catch
            {
                // Se não conseguir buscar as configurações, usa o padrão
            }

            var start = dateTime;
            var end = dateTime.AddMinutes(durationMinutes);

            var queryable = await Repository.GetQueryableAsync();

            // Verifica conflito para o paciente
            var patientConflict = queryable.Any(c =>
                c.PatientId == patientId &&
                c.DateTime < end &&
                c.DateTime.AddMinutes(durationMinutes) > start);

            if (patientConflict)
            {
                throw new UserFriendlyException(
                    "Conflito de horário: o paciente já possui uma consulta agendada neste horário.");
            }

            // Verifica conflito para o terapeuta
            var therapistConflict = queryable.Any(c =>
                c.TherapistId == therapistId &&
                c.DateTime < end &&
                c.DateTime.AddMinutes(durationMinutes) > start);

            if (therapistConflict)
            {
                throw new UserFriendlyException(
                    "Conflito de horário: o terapeuta já possui uma consulta agendada neste horário.");
            }
        }

        #region Métodos Originais Mantidos
        public override async Task<ConsultationDto> GetAsync(Guid id)
        {
            // Para carregar os treinos junto com a consulta, adicione .WithDetailsAsync() aqui também
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist, x => x.PerformedTrainings);
            var query = queryable.Where(x => x.Id == id);
            var consultation = await AsyncExecuter.FirstOrDefaultAsync(query);
            if (consultation == null)
            {
                throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Consultation), id);
            }
            return ObjectMapper.Map<Consultation, ConsultationDto>(consultation);
        }

        public override async Task<PagedResultDto<ConsultationDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var queryable = await Repository.WithDetailsAsync(x => x.Patient, x => x.Therapist);

            // Filtro de Segurança por Papel (Role)
            if (CurrentUser.IsInRole("Responsible"))
            {
                var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
                if (responsible != null)
                {
                    // Mostrar apenas consultas dos pacientes sob responsabilidade deste usuário
                    queryable = queryable.Where(x => x.Patient.PrincipalResponsibleId == responsible.Id);
                }
                else
                {
                    queryable = queryable.Where(x => false);
                }
            }
            else if (CurrentUser.IsInRole("Guest"))
            {
                var guest = await _guestRepository.FirstOrDefaultAsync(g => g.UserId == CurrentUser.Id);
                if (guest != null)
                {
                    // Mostrar apenas consultas do paciente vinculado a este convidado
                    queryable = queryable.Where(x => x.PatientId == guest.PatientId);
                }
                else
                {
                    queryable = queryable.Where(x => false);
                }
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            queryable = ApplySorting(queryable, input);
            queryable = ApplyPaging(queryable, input);

            var consultations = await AsyncExecuter.ToListAsync(queryable);

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

        #region Sessão em Tempo Real
        [Authorize(WeCarePermissions.Consultations.Edit)]
        public async Task<ConsultationDto> CompleteSessionAsync(Guid consultationId, CreateUpdateConsultationDto input)
        {
            var queryable = await Repository.WithDetailsAsync(x => x.PerformedTrainings);
            var consultation = await AsyncExecuter.FirstOrDefaultAsync(
                queryable.Where(x => x.Id == consultationId));

            if (consultation == null)
            {
                throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Consultation), consultationId);
            }

            if (consultation.Status != ConsultationStatus.Agendada)
            {
                throw new UserFriendlyException("Esta consulta já foi realizada.");
            }

            // Atualizar campos da sessão
            consultation.Status = ConsultationStatus.Realizada;
            consultation.ObjectiveId = input.ObjectiveId;
            consultation.Description = input.Description;
            consultation.MainTraining = input.MainTraining;
            consultation.Duration = input.Duration;

            // Limpar treinos antigos e adicionar os novos
            consultation.PerformedTrainings.Clear();
            foreach (var trainingDto in input.PerformedTrainings)
            {
                var training = ObjectMapper.Map<CreateUpdatePerformedTrainingDto, PerformedTraining>(trainingDto);
                consultation.PerformedTrainings.Add(training);
            }

            await Repository.UpdateAsync(consultation, autoSave: true);

            return ObjectMapper.Map<Consultation, ConsultationDto>(consultation);
        }
        #endregion
    }
}