using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
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
using WeCare.Domain.Shared.PerformedTrainings;

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
        private readonly IGuidGenerator _guidGenerator;

        public WeCareTestDataSeederContributor(
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Therapist, Guid> therapistRepository,
            IRepository<Objective, Guid> objectiveRepository,
            IRepository<Consultation, Guid> consultationRepository,
            IRepository<Training, Guid> trainingRepository,
            IRepository<PerformedTraining, Guid> performedTrainingRepository,
            IRepository<Clinic, Guid> clinicRepository,
            IGuidGenerator guidGenerator)
        {
            _responsibleRepository = responsibleRepository;
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;
            _objectiveRepository = objectiveRepository;
            _consultationRepository = consultationRepository;
            _trainingRepository = trainingRepository;
            _performedTrainingRepository = performedTrainingRepository;
            _clinicRepository = clinicRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Verificar se já existem dados
            if (await _patientRepository.GetCountAsync() > 0)
            {
                // return; // Já existem dados, não semear novamente
            }

            // 0. Criar Clínica WeCare (Host)
            if (await _clinicRepository.CountAsync(x => x.Name == "WeCare") == 0)
            {
                var weCareClinic = new Clinic
                {
                    Name = "WeCare",
                    Email = "admin@wecare.com.br",
                    Address = "Sede WeCare",
                    Status = ClinicStatus.Active,
                    TenantId = null // Host
                };
                await _clinicRepository.InsertAsync(weCareClinic, autoSave: true);
            }

            // 1. Criar Responsáveis
            var responsible1 = new Responsible
            {
                CPF = "12345678901",
                NameResponsible = "Maria Silva Santos",
                EmailAddress = "maria.santos@email.com",
                PhoneNumber = "(11) 98765-4321"
            };

            var responsible2 = new Responsible
            {
                CPF = "98765432100",
                NameResponsible = "João Pedro Oliveira",
                EmailAddress = "joao.oliveira@email.com",
                PhoneNumber = "(11) 97654-3210"
            };

            var responsible3 = new Responsible
            {
                CPF = "45678912300",
                NameResponsible = "Ana Carolina Costa",
                EmailAddress = "ana.costa@email.com",
                PhoneNumber = "(21) 99876-5432"
            };

            responsible1 = await _responsibleRepository.InsertAsync(responsible1, autoSave: true);
            responsible2 = await _responsibleRepository.InsertAsync(responsible2, autoSave: true);
            responsible3 = await _responsibleRepository.InsertAsync(responsible3, autoSave: true);

            // 2. Criar Pacientes
            var patient1 = new Patient
            {
                Name = "Lucas Silva Santos",
                BirthDate = new DateTime(2015, 3, 15),
                Address = "Rua das Flores, 123 - São Paulo, SP",
                Diag = "TEA - Transtorno do Espectro Autista",
                PrincipalResponsibleId = responsible1.Id
            };

            var patient2 = new Patient
            {
                Name = "Gabriela Oliveira",
                BirthDate = new DateTime(2016, 7, 22),
                Address = "Av. Paulista, 456 - São Paulo, SP",
                Diag = "TDAH - Transtorno de Déficit de Atenção",
                PrincipalResponsibleId = responsible2.Id
            };

            var patient3 = new Patient
            {
                Name = "Pedro Costa",
                BirthDate = new DateTime(2014, 11, 8),
                Address = "Rua Copacabana, 789 - Rio de Janeiro, RJ",
                Diag = "Atraso no Desenvolvimento",
                PrincipalResponsibleId = responsible3.Id
            };

            var patient4 = new Patient
            {
                Name = "Sofia Santos",
                BirthDate = new DateTime(2017, 5, 30),
                Address = "Rua das Flores, 123 - São Paulo, SP",
                Diag = "TEA Leve",
                PrincipalResponsibleId = responsible1.Id
            };

            patient1 = await _patientRepository.InsertAsync(patient1, autoSave: true);
            patient2 = await _patientRepository.InsertAsync(patient2, autoSave: true);
            patient3 = await _patientRepository.InsertAsync(patient3, autoSave: true);
            patient4 = await _patientRepository.InsertAsync(patient4, autoSave: true);

            // 3. Criar Terapeutas (usando um GUID fixo para UserId - você pode ajustar conforme necessário)
            var therapist1 = new Therapist
            {
                Name = "Dra. Camila Rodrigues",
                Email = "camila.rodrigues@wecare.com",
                UserId = Guid.NewGuid(), // Aqui você precisaria usar o ID de um usuário real
                Specialization = "Terapia Ocupacional"
            };

            var therapist2 = new Therapist
            {
                Name = "Dr. Rafael Mendes",
                Email = "rafael.mendes@wecare.com",
                UserId = Guid.NewGuid(),
                Specialization = "Fonoaudiologia"
            };

            var therapist3 = new Therapist
            {
                Name = "Dra. Patricia Lima",
                Email = "patricia.lima@wecare.com",
                UserId = Guid.NewGuid(),
                Specialization = "Psicologia Infantil"
            };

            therapist1 = await _therapistRepository.InsertAsync(therapist1, autoSave: true);
            therapist2 = await _therapistRepository.InsertAsync(therapist2, autoSave: true);
            therapist3 = await _therapistRepository.InsertAsync(therapist3, autoSave: true);

            // 4. Criar Objetivos
            var objective1 = new Objective
            {
                PatientId = patient1.Id,
                TherapistId = therapist1.Id,
                Name = "Melhorar coordenação motora fina",
                Status = "Ativo",
                StartDate = DateTime.Now.AddMonths(-3)
            };

            var objective2 = new Objective
            {
                PatientId = patient1.Id,
                TherapistId = therapist2.Id,
                Name = "Desenvolver comunicação verbal",
                Status = "Ativo",
                StartDate = DateTime.Now.AddMonths(-2)
            };

            var objective3 = new Objective
            {
                PatientId = patient2.Id,
                TherapistId = therapist3.Id,
                Name = "Aumentar tempo de concentração",
                Status = "Ativo",
                StartDate = DateTime.Now.AddMonths(-1)
            };

            var objective4 = new Objective
            {
                PatientId = patient3.Id,
                TherapistId = therapist1.Id,
                Name = "Fortalecer habilidades sociais",
                Status = "Concluído",
                StartDate = DateTime.Now.AddMonths(-6),
                EndDate = DateTime.Now.AddMonths(-1)
            };

            objective1 = await _objectiveRepository.InsertAsync(objective1, autoSave: true);
            objective2 = await _objectiveRepository.InsertAsync(objective2, autoSave: true);
            objective3 = await _objectiveRepository.InsertAsync(objective3, autoSave: true);
            objective4 = await _objectiveRepository.InsertAsync(objective4, autoSave: true);

            // 5. Criar Atividades (Trainings)
            var training1 = new Training
            {
                Name = "Encaixe de formas geométricas",
                Description = "Atividade para trabalhar coordenação motora e reconhecimento de formas",
                ObjectiveId = objective1.Id
            };

            var training2 = new Training
            {
                Name = "Pintura com os dedos",
                Description = "Estimular sensibilidade tátil e coordenação",
                ObjectiveId = objective1.Id
            };

            var training3 = new Training
            {
                Name = "Repetição de sons",
                Description = "Exercícios de fonética básica",
                ObjectiveId = objective2.Id
            };

            var training4 = new Training
            {
                Name = "Jogos de memória - 5 minutos",
                Description = "Atividade focada em manter atenção por períodos curtos",
                ObjectiveId = objective3.Id
            };

            training1 = await _trainingRepository.InsertAsync(training1, autoSave: true);
            training2 = await _trainingRepository.InsertAsync(training2, autoSave: true);
            training3 = await _trainingRepository.InsertAsync(training3, autoSave: true);
            training4 = await _trainingRepository.InsertAsync(training4, autoSave: true);

            // 6. Criar Consultas
            var consultation1 = new Consultation
            {
                PatientId = patient1.Id,
                TherapistId = therapist1.Id,
                ObjectiveId = objective1.Id,
                DateTime = DateTime.Now.AddDays(-7),
                Description = "Sessão focada em coordenação motora. Lucas mostrou progresso significativo.",
                Specialty = "Terapia Ocupacional",
                MainTraining = "Encaixe de formas",
                Duration = "45 minutos"
            };

            var consultation2 = new Consultation
            {
                PatientId = patient1.Id,
                TherapistId = therapist2.Id,
                ObjectiveId = objective2.Id,
                DateTime = DateTime.Now.AddDays(-5),
                Description = "Trabalho de fonemas. Boa participação do paciente.",
                Specialty = "Fonoaudiologia",
                MainTraining = "Repetição de sons",
                Duration = "40 minutos"
            };

            var consultation3 = new Consultation
            {
                PatientId = patient2.Id,
                TherapistId = therapist3.Id,
                ObjectiveId = objective3.Id,
                DateTime = DateTime.Now.AddDays(-3),
                Description = "Gabriela conseguiu manter foco por 7 minutos. Excelente evolução!",
                Specialty = "Psicologia Infantil",
                MainTraining = "Jogos de memória",
                Duration = "50 minutos"
            };

            var consultation4 = new Consultation
            {
                PatientId = patient1.Id,
                TherapistId = therapist1.Id,
                ObjectiveId = objective1.Id,
                DateTime = DateTime.Now.AddDays(-1),
                Description = "Continuação do trabalho com coordenação. Introduzido novo material.",
                Specialty = "Terapia Ocupacional",
                MainTraining = "Pintura com dedos",
                Duration = "45 minutos"
            };

            consultation1 = await _consultationRepository.InsertAsync(consultation1, autoSave: true);
            consultation2 = await _consultationRepository.InsertAsync(consultation2, autoSave: true);
            consultation3 = await _consultationRepository.InsertAsync(consultation3, autoSave: true);
            consultation4 = await _consultationRepository.InsertAsync(consultation4, autoSave: true);

            // 7. Criar Treinamentos Realizados (PerformedTrainings)
            var performedTraining1 = new PerformedTraining
            {
                TrainingId = training1.Id,
                ConsultationId = consultation1.Id,
                HelpNeeded = HelpNeededType.I, // Independente
                TotalAttempts = 10,
                SuccessfulAttempts = 8
            };

            var performedTraining2 = new PerformedTraining
            {
                TrainingId = training3.Id,
                ConsultationId = consultation2.Id,
                HelpNeeded = HelpNeededType.SV, // Suporte Verbal
                TotalAttempts = 15,
                SuccessfulAttempts = 12
            };

            var performedTraining3 = new PerformedTraining
            {
                TrainingId = training4.Id,
                ConsultationId = consultation3.Id,
                HelpNeeded = HelpNeededType.SG, // Suporte Gestual
                TotalAttempts = 5,
                SuccessfulAttempts = 4
            };

            var performedTraining4 = new PerformedTraining
            {
                TrainingId = training2.Id,
                ConsultationId = consultation4.Id,
                HelpNeeded = HelpNeededType.SP, // Suporte Posicional
                TotalAttempts = 8,
                SuccessfulAttempts = 7
            };

            await _performedTrainingRepository.InsertAsync(performedTraining1, autoSave: true);
            await _performedTrainingRepository.InsertAsync(performedTraining2, autoSave: true);
            await _performedTrainingRepository.InsertAsync(performedTraining3, autoSave: true);
            await _performedTrainingRepository.InsertAsync(performedTraining4, autoSave: true);
        }
    }
}
