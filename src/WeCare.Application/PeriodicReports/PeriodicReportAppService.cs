using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using WeCare.Consultations;
using WeCare.Objectives;
using WeCare.Patients;
using WeCare.Permissions;
using WeCare.Shared;
using WeCare.Therapists;
using WeCare.Tratamentos;

namespace WeCare.PeriodicReports
{
    [Authorize(WeCarePermissions.PeriodicReports.Default)]
    public class PeriodicReportAppService : CrudAppService<
            PeriodicReport,
            PeriodicReportDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateUpdatePeriodicReportDto>,
        IPeriodicReportAppService
    {
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Therapist, Guid> _therapistRepository;
        private readonly IRepository<Consultation, Guid> _consultationRepository;
        private readonly IRepository<Objective, Guid> _objectiveRepository;
        private readonly IRepository<Tratamento, Guid> _tratamentoRepository;

        public PeriodicReportAppService(
            IRepository<PeriodicReport, Guid> repository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Therapist, Guid> therapistRepository,
            IRepository<Consultation, Guid> consultationRepository,
            IRepository<Objective, Guid> objectiveRepository,
            IRepository<Tratamento, Guid> tratamentoRepository)
            : base(repository)
        {
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;
            _consultationRepository = consultationRepository;
            _objectiveRepository = objectiveRepository;
            _tratamentoRepository = tratamentoRepository;

            GetPolicyName = WeCarePermissions.PeriodicReports.Default;
            GetListPolicyName = WeCarePermissions.PeriodicReports.Default;
            CreatePolicyName = WeCarePermissions.PeriodicReports.Create;
            UpdatePolicyName = WeCarePermissions.PeriodicReports.Edit;
            DeletePolicyName = WeCarePermissions.PeriodicReports.Delete;
        }

        public async Task<List<LookupDto<Guid>>> GetPatientLookupAsync()
        {
            var patients = await _patientRepository.GetListAsync();
            return patients.Select(p => new LookupDto<Guid>(p.Id, p.Name)).ToList();
        }

        public async Task<List<LookupDto<Guid>>> GetTherapistLookupAsync()
        {
            var therapists = await _therapistRepository.GetListAsync();
            return therapists.Select(t => new LookupDto<Guid>(t.Id, t.Name)).ToList();
        }

        public override async Task<PeriodicReportDto> GetAsync(Guid id)
        {
            var report = await Repository.GetAsync(id);
            var dto = ObjectMapper.Map<PeriodicReport, PeriodicReportDto>(report);

            var patient = await _patientRepository.FindAsync(report.PatientId);
            if (patient != null)
            {
                dto.PatientName = patient.Name;
            }

            var therapist = await _therapistRepository.FindAsync(report.TherapistId);
            if (therapist != null)
            {
                dto.TherapistName = therapist.Name;
            }

            return dto;
        }

        public override async Task<PagedResultDto<PeriodicReportDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            if (input.Sorting.IsNullOrWhiteSpace())
            {
                input.Sorting = "CreationTime DESC";
            }

            var queryable = await Repository.GetQueryableAsync();
            var totalCount = await AsyncExecuter.CountAsync(queryable);

            queryable = ApplySorting(queryable, input);
            queryable = ApplyPaging(queryable, input);

            var reports = await AsyncExecuter.ToListAsync(queryable);
            var reportDtos = ObjectMapper.Map<List<PeriodicReport>, List<PeriodicReportDto>>(reports);

            if (reportDtos.Any())
            {
                var patientIds = reportDtos.Select(r => r.PatientId).Distinct().ToList();
                var patients = await _patientRepository.GetListAsync(p => patientIds.Contains(p.Id));
                var patientDict = patients.ToDictionary(p => p.Id, p => p.Name);

                var therapistIds = reportDtos.Select(r => r.TherapistId).Distinct().ToList();
                var therapists = await _therapistRepository.GetListAsync(t => therapistIds.Contains(t.Id));
                var therapistDict = therapists.ToDictionary(t => t.Id, t => t.Name);

                foreach (var dto in reportDtos)
                {
                    if (patientDict.TryGetValue(dto.PatientId, out var patientName))
                    {
                        dto.PatientName = patientName;
                    }

                    if (therapistDict.TryGetValue(dto.TherapistId, out var therapistName))
                    {
                        dto.TherapistName = therapistName;
                    }
                }
            }

            return new PagedResultDto<PeriodicReportDto>(
                totalCount,
                reportDtos
            );
        }

        public async Task<PeriodicReportDto> GetDraftReportAsync(Guid patientId, DateTime startDate, DateTime endDate)
        {
            var patient = await _patientRepository.GetAsync(patientId);

            // Tenta obter o terapeuta associado ao paciente por meio de consultas ou tratamentos
            Guid therapistId = Guid.Empty;
            string therapistName = string.Empty;

            var consultations = await _consultationRepository.GetListAsync(c =>
                c.PatientId == patientId &&
                c.DateTime >= startDate &&
                c.DateTime <= endDate);

            if (consultations.Any())
            {
                var firstConsultation = consultations.First();
                therapistId = firstConsultation.TherapistId;
            }
            else
            {
                var treatments = await _tratamentoRepository.GetListAsync(t => t.PatientId == patientId);
                if (treatments.Any())
                {
                    therapistId = treatments.First().TherapistId;
                }
                else
                {
                    var therapists = await _therapistRepository.GetListAsync();
                    if (therapists.Any())
                    {
                        therapistId = therapists.First().Id;
                    }
                    else
                    {
                        throw new UserFriendlyException("Não foi possível encontrar nenhum terapeuta cadastrado no sistema.");
                    }
                }
            }

            var therapistObj = await _therapistRepository.GetAsync(therapistId);
            therapistName = therapistObj.Name;

            // Pre-popula o Resumo Clínico com as anotações diárias das consultas
            string resumoClinico = "Não há consultas registradas no período selecionado.";
            if (consultations.Any())
            {
                var notes = consultations
                    .Where(c => !string.IsNullOrWhiteSpace(c.Description))
                    .Select(c => $"- {c.Description} ({c.DateTime:dd/MM/yyyy})");

                if (notes.Any())
                {
                    resumoClinico = string.Join("\n", notes);
                }
            }

            // Mapeia objetivos
            var objectives = await _objectiveRepository.GetListAsync(o => o.PatientId == patientId);
            var objectivesList = objectives.Select(o => $"{{\"Objective\":\"{o.Name}\",\"Status\":\"Iniciando\"}}");
            var objetivosJson = "[" + string.Join(",", objectivesList) + "]";

            var draftDto = new PeriodicReportDto
            {
                PatientId = patientId,
                PatientName = patient.Name,
                TherapistId = therapistId,
                TherapistName = therapistName,
                Title = $"Relatório Periódico de {patient.Name}",
                StartDate = startDate,
                EndDate = endDate,
                ResumoClinico = resumoClinico,
                ObjetivosStatus = objetivosJson,
                EngajamentoCasa = "",
                ProximosPassos = "",
                Status = PeriodicReportStatus.Draft
            };

            return draftDto;
        }

        public async Task<PeriodicReportDto> SignAndPublishAsync(Guid id)
        {
            var report = await Repository.GetAsync(id);
            if (report.Status != PeriodicReportStatus.Draft)
            {
                throw new UserFriendlyException("Apenas relatórios em rascunho podem ser publicados.");
            }

            report.Status = PeriodicReportStatus.Published;
            await Repository.UpdateAsync(report, autoSave: true);

            return await GetAsync(report.Id);
        }

        public async Task<PeriodicReportDto> ParentSignAsync(Guid id, ParentSignReportDto input)
        {
            var report = await Repository.GetAsync(id);
            if (report.Status != PeriodicReportStatus.Published)
            {
                throw new UserFriendlyException("Apenas relatórios publicados e aguardando ciente podem ser assinados.");
            }

            report.ResponsibleSignatureCPF = input.ResponsibleSignatureCPF;
            report.ResponsibleSignatureIP = input.ResponsibleSignatureIP;
            report.ResponsibleSignatureDate = DateTime.Now;

            // Calcula o hash de verificação digital SHA-256
            string rawData = $"{report.Id}|{input.ResponsibleSignatureCPF}|{input.ResponsibleSignatureIP}|{report.ResumoClinico}";
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                report.ParentSignatureHash = sb.ToString();
            }

            report.Status = PeriodicReportStatus.SignedByResponsible;
            await Repository.UpdateAsync(report, autoSave: true);

            return await GetAsync(report.Id);
        }
    }
}
