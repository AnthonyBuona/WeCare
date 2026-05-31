using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using WeCare.Tratamentos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using WeCare.Patients;
using WeCare.Therapists;
using WeCare.Responsibles;
using WeCare.Objectives;
using WeCare.Consultations;
using WeCare.Trainings;
using WeCare.PerformedTrainings;
using WeCare.Clinics;
using WeCare.Attendances;
using WeCare.Domain.Shared.PerformedTrainings;
using Volo.Abp.Identity;
using Volo.Abp.TenantManagement;
using Volo.Abp.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using WeCare.PeriodicReports;
using WeCare.CrossTenantAccess;
using WeCare.Billing;
using WeCare.Gamification;

namespace WeCare.Data
{
    public class WeCareTestDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Therapist, Guid> _therapistRepository;
        private readonly IRepository<Objective, Guid> _objectiveRepository;
        private readonly IRepository<Consultation, Guid> _consultationRepository;
        private readonly IRepository<Training, Guid> _trainingRepository;
        private readonly IRepository<PerformedTraining, Guid> _performedTrainingRepository;
        private readonly IRepository<Clinic, Guid> _clinicRepository;
        private readonly IRepository<Tratamento, Guid> _tratamentoRepository;
        private readonly IRepository<Attendance, Guid> _attendanceRepository;
        private readonly IRepository<PeriodicReport, Guid> _periodicReportRepository;
        private readonly IRepository<CrossTenantAccessConsent, Guid> _consentRepository;
        private readonly IRepository<SharedAccessAuditLog, Guid> _auditLogRepository;
        private readonly IRepository<TussProcedureMapping, Guid> _tussMappingRepository;
        private readonly IRepository<BillingGuide, Guid> _billingGuideRepository;
        private readonly IRepository<BillingBatch, Guid> _billingBatchRepository;
        private readonly IRepository<CaregiverQuest, Guid> _questRepository;
        private readonly IRepository<QuestExecutionLog, Guid> _questLogRepository;
        private readonly IRepository<UserGamifiedProfile, Guid> _gamifiedProfileRepository;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IdentityUserManager _userManager;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITenantManager _tenantManager;
        private readonly ICurrentTenant _currentTenant;

        public WeCareTestDataSeederContributor(
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Therapist, Guid> therapistRepository,
            IRepository<Objective, Guid> objectiveRepository,
            IRepository<Consultation, Guid> consultationRepository,
            IRepository<Training, Guid> trainingRepository,
            IRepository<PerformedTraining, Guid> performedTrainingRepository,
            IRepository<Clinic, Guid> clinicRepository,
            IRepository<Tratamento, Guid> tratamentoRepository,
            IRepository<Attendance, Guid> attendanceRepository,
            IRepository<PeriodicReport, Guid> periodicReportRepository,
            IRepository<CrossTenantAccessConsent, Guid> consentRepository,
            IRepository<SharedAccessAuditLog, Guid> auditLogRepository,
            IRepository<TussProcedureMapping, Guid> tussMappingRepository,
            IRepository<BillingGuide, Guid> billingGuideRepository,
            IRepository<BillingBatch, Guid> billingBatchRepository,
            IRepository<CaregiverQuest, Guid> questRepository,
            IRepository<QuestExecutionLog, Guid> questLogRepository,
            IRepository<UserGamifiedProfile, Guid> gamifiedProfileRepository,
            IGuidGenerator guidGenerator,
            IdentityUserManager userManager,
            ITenantRepository tenantRepository,
            ITenantManager tenantManager,
            ICurrentTenant currentTenant)
        {
            _responsibleRepository = responsibleRepository;
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;
            _objectiveRepository = objectiveRepository;
            _consultationRepository = consultationRepository;
            _trainingRepository = trainingRepository;
            _performedTrainingRepository = performedTrainingRepository;
            _clinicRepository = clinicRepository;
            _tratamentoRepository = tratamentoRepository;
            _attendanceRepository = attendanceRepository;
            _periodicReportRepository = periodicReportRepository;
            _consentRepository = consentRepository;
            _auditLogRepository = auditLogRepository;
            _tussMappingRepository = tussMappingRepository;
            _billingGuideRepository = billingGuideRepository;
            _billingBatchRepository = billingBatchRepository;
            _questRepository = questRepository;
            _questLogRepository = questLogRepository;
            _gamifiedProfileRepository = gamifiedProfileRepository;
            _guidGenerator = guidGenerator;
            _userManager = userManager;
            _tenantRepository = tenantRepository;
            _tenantManager = tenantManager;
            _currentTenant = currentTenant;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (context.TenantId == null)
            {
                await SeedHostDataAsync();
            }
            else
            {
                using (_currentTenant.Change(context.TenantId))
                {
                    await SeedTenantDataAsync(context.TenantId.Value);
                }
            }
        }

        private async Task SeedHostDataAsync()
        {
            // 1. Criar Tenant se não existir
            if (await _tenantRepository.FindByNameAsync("ClinicaBemViver") == null)
            {
                var tenant = await _tenantManager.CreateAsync("ClinicaBemViver");
                await _tenantRepository.InsertAsync(tenant, autoSave: true);
            }

            // 2. Criar Equipe Técnica no Host (Painel Admin)
            await CreateUserAsync("suporte@wecare.com", "1q2w3E*", "admin");
        }

        private async Task SeedTenantDataAsync(Guid tenantId)
        {
            if (await _patientRepository.GetCountAsync() > 0) return;

            // Buscar usuários criados no host ou criar novos para o tenant se necessário
            // Para simplificar, vamos criar usuários locais no tenant
            var therapistUser = await CreateUserAsync($"terapeuta@{tenantId.ToString().Substring(0, 8)}.com", "1q2w3E*", "Therapist");
            var responsibleUser = await CreateUserAsync($"responsavel@{tenantId.ToString().Substring(0, 8)}.com", "1q2w3E*", "Responsible");

            // 1. Responsável
            var responsible = await _responsibleRepository.InsertAsync(new Responsible
            {
                CPF = "12345678901",
                NameResponsible = "Maria Responsável",
                EmailAddress = responsibleUser.Email,
                UserId = responsibleUser.Id
            }, autoSave: true);

            // 2. Terapeuta
            var therapist = await _therapistRepository.InsertAsync(new Therapist
            {
                Name = "Dra. Camila Terapeuta",
                Email = therapistUser.Email,
                UserId = therapistUser.Id,
                Specialization = "Terapia Ocupacional"
            }, autoSave: true);

            // 3. Paciente
            var patient = await _patientRepository.InsertAsync(new Patient
            {
                Name = "Lucas Paciente",
                BirthDate = DateTime.Now.AddYears(-8),
                PrincipalResponsibleId = responsible.Id
            }, autoSave: true);

            // 4. Tratamento
            var tratamento = await _tratamentoRepository.InsertAsync(new Tratamento
            {
                PatientId = patient.Id,
                TherapistId = therapist.Id,
                Tipo = "Psicopedagogia"
            }, autoSave: true);

            // 5. Objetivo
            var objective = await _objectiveRepository.InsertAsync(new Objective
            {
                PatientId = patient.Id,
                TherapistId = therapist.Id,
                Name = "Desenvolvimento Motor",
                Status = "Ativo",
                StartDate = DateTime.Now.AddMonths(-1)
            }, autoSave: true);

            // 5. Treinamento
            var training = await _trainingRepository.InsertAsync(new Training
            {
                Name = "Coordenação Fina",
                Description = "Exercícios para fortalecer a coordenação motora fina.",
                ObjectiveId = objective.Id
            }, autoSave: true);

            // 7. Consulta
            var consultation = await _consultationRepository.InsertAsync(new Consultation
            {
                PatientId = patient.Id,
                TherapistId = therapist.Id,
                ObjectiveId = objective.Id,
                TratamentoId = tratamento.Id,
                DateTime = DateTime.Now.AddDays(-1),
                Description = "Sessão inicial produtiva.",
                Duration = "50 min",
                Status = ConsultationStatus.Realizada,
                Specialty = "Terapia Ocupacional",
                MainTraining = "Coordenação Fina"
            }, autoSave: true);

            // 8. Attendances for Lucas Paciente
            await _attendanceRepository.InsertAsync(new Attendance(
                _guidGenerator.Create(),
                patient.Id,
                DateTime.Now.AddDays(-2),
                AttendanceStatus.Present,
                "Present and engaged",
                tenantId
            ), autoSave: true);

            await _attendanceRepository.InsertAsync(new Attendance(
                _guidGenerator.Create(),
                patient.Id,
                DateTime.Now.AddDays(-5),
                AttendanceStatus.Absent,
                "Absent without justification",
                tenantId
            ), autoSave: true);

            await _attendanceRepository.InsertAsync(new Attendance(
                _guidGenerator.Create(),
                patient.Id,
                DateTime.Now.AddDays(-9),
                AttendanceStatus.Present,
                "Present and very collaborative",
                tenantId
            ), autoSave: true);

            if (await _periodicReportRepository.GetCountAsync() == 0)
            {
                // 1. Rascunho (Draft)
                await _periodicReportRepository.InsertAsync(new PeriodicReport(
                    _guidGenerator.Create(),
                    patient.Id,
                    therapist.Id,
                    "Lucas demonstrou excelente evolução motora fina durante o último bimestre. Sua participação nas dinâmicas de coordenação motora foi exemplar.",
                    "[{\"Objective\":\"Coordenação Fina\",\"Status\":\"Em Progresso\"}]",
                    "A família de Lucas tem sido muito solícita, aplicando os treinos de coordenação em casa conforme orientado.",
                    "Focar em habilidades de escrita e pinça fina nas próximas sessões.",
                    PeriodicReportStatus.Draft,
                    tenantId
                ), autoSave: true);

                // 2. Publicado (Published - Pendente Assinatura)
                await _periodicReportRepository.InsertAsync(new PeriodicReport(
                    _guidGenerator.Create(),
                    patient.Id,
                    therapist.Id,
                    "Relatório Periódico Bimestral (Abril/Maio)",
                    DateTime.Now.AddMonths(-2),
                    DateTime.Now,
                    "Paciente evoluiu consideravelmente na fala e na expressividade comunicativa. O treino em consultório foca em fonemas complexos e contato visual.",
                    "[{\"Objective\":\"Contato Visual\",\"Status\":\"Em Progresso\"},{\"Objective\":\"Socialização\",\"Status\":\"Iniciando\"}]",
                    "Engajamento familiar nota 10. Carlos tem praticado dinâmicas de jogos em dupla no lar.",
                    "Introduzir mais exercícios de fala expressiva no próximo período.",
                    PeriodicReportStatus.Published,
                    null,
                    null,
                    null,
                    null,
                    tenantId
                ), autoSave: true);

                // 3. Assinado (Signed - Histórico)
                await _periodicReportRepository.InsertAsync(new PeriodicReport(
                    _guidGenerator.Create(),
                    patient.Id,
                    therapist.Id,
                    "Avaliação Periódica Integrada (Fevereiro/Março)",
                    DateTime.Now.AddMonths(-4),
                    DateTime.Now.AddMonths(-2),
                    "Lucas teve excelente adaptação ao ambiente clínico e avanços no brincar compartilhado. Reduziu crises de choro e frustração frente a limites.",
                    "[{\"Objective\":\"Auto-regulação\",\"Status\":\"Conquistado\"},{\"Objective\":\"Socialização\",\"Status\":\"Em Progresso\"}]",
                    "Acompanhamento em casa foi de suma relevância para consolidação de limites.",
                    "Fortalecer interações sociais na escola nas próximas semanas.",
                    PeriodicReportStatus.SignedByResponsible,
                    DateTime.Now.AddMonths(-2),
                    "192.168.1.102",
                    "123.456.789-01",
                    "d8a57e3f890e0c89abddd8e244eccf5678abdd7e907b22a8bcff29f12345678a",
                    tenantId
                ), autoSave: true);
            }

            // --- Features 2, 3, 4 Seed Data ---
            if (await _consentRepository.GetCountAsync() == 0)
            {
                var targetTenantId = _guidGenerator.Create();

                var consent = await _consentRepository.InsertAsync(new CrossTenantAccessConsent(
                    _guidGenerator.Create(),
                    patient.Id,
                    tenantId,
                    targetTenantId,
                    DateTime.Now.AddDays(30),
                    "Read",
                    "d8a57e3f890e0c89abddd8e244eccf5678abdd7e907b22a8bcff29f12345678a",
                    false,
                    tenantId
                ), autoSave: true);

                await _auditLogRepository.InsertAsync(new SharedAccessAuditLog(
                    _guidGenerator.Create(),
                    consent.Id,
                    therapistUser.Id,
                    "Viewed multidisciplinary timeline",
                    DateTime.Now.AddMinutes(-5),
                    "192.168.1.105",
                    tenantId
                ), autoSave: true);
            }

            if (await _tussMappingRepository.GetCountAsync() == 0)
            {
                await _tussMappingRepository.InsertAsync(new TussProcedureMapping(
                    _guidGenerator.Create(),
                    "Terapia Ocupacional",
                    "50000470",
                    "Fonoaudiologia individual"
                ), autoSave: true);

                await _tussMappingRepository.InsertAsync(new TussProcedureMapping(
                    _guidGenerator.Create(),
                    "Psicopedagogia",
                    "50000488",
                    "Psicopedagogia individual"
                ), autoSave: true);
            }

            if (await _billingGuideRepository.GetCountAsync() == 0)
            {
                await _billingGuideRepository.InsertAsync(new BillingGuide(
                    _guidGenerator.Create(),
                    consultation.Id,
                    "Amil",
                    "1234567890123456",
                    "PASS123",
                    150.00m,
                    "Pending",
                    tenantId
                ), autoSave: true);
            }

            if (await _billingBatchRepository.GetCountAsync() == 0)
            {
                await _billingBatchRepository.InsertAsync(new BillingBatch(
                    _guidGenerator.Create(),
                    "BATCH-2026-001",
                    DateTime.Now,
                    "<xml><lote>1</lote></xml>",
                    "BATCH_HASH_SIGNATURE_EX",
                    tenantId
                ), autoSave: true);
            }

            if (await _questRepository.GetCountAsync() == 0)
            {
                var quest = await _questRepository.InsertAsync(new CaregiverQuest(
                    _guidGenerator.Create(),
                    patient.Id,
                    "Praticar abotoar camisa",
                    "Praticar abotoar a camisa por 3 minutos com a criança.",
                    "https://wecare.blob.core.windows.net/videos/quest1.mp4",
                    50,
                    tenantId
                ), autoSave: true);

                await _questLogRepository.InsertAsync(new QuestExecutionLog(
                    _guidGenerator.Create(),
                    quest.Id,
                    DateTime.Now.AddDays(-1),
                    5,
                    "Lucas gostou muito e conseguiu abotoar 3 botões sozinho!",
                    tenantId
                ), autoSave: true);
            }

            if (await _gamifiedProfileRepository.GetCountAsync() == 0)
            {
                await _gamifiedProfileRepository.InsertAsync(new UserGamifiedProfile(
                    _guidGenerator.Create(),
                    responsibleUser.Id,
                    2,
                    150,
                    3,
                    "[\"PrimeiroTreino\", \"FamiliaEngajada\"]",
                    tenantId
                ), autoSave: true);
            }
        }


        private async Task<Volo.Abp.Identity.IdentityUser> CreateUserAsync(string email, string password, string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new Volo.Abp.Identity.IdentityUser(_guidGenerator.Create(), email, email, _currentTenant.Id);
                CheckErrors(await _userManager.CreateAsync(user, password));
                CheckErrors(await _userManager.AddToRoleAsync(user, role));
            }
            return user;
        }

        private void CheckErrors(Microsoft.AspNetCore.Identity.IdentityResult result)
        {
            if (!result.Succeeded)
            {
                throw new Exception("Identity error: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
