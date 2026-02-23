using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using WeCare.Patients;
using WeCare.Therapists;
using WeCare.Clinics;
using WeCare.Objectives;
using WeCare.Consultations;
using WeCare.Responsibles;
using WeCare.Trainings;
using WeCare.Guests;
using Volo.Abp.Users;
using Microsoft.AspNetCore.Authorization;

namespace WeCare.Dashboards
{
    [Authorize]
    public class DashboardAppService : ApplicationService, IDashboardAppService
    {
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Therapist, Guid> _therapistRepository;
        private readonly IRepository<Clinic, Guid> _clinicRepository;
        private readonly IRepository<Consultation, Guid> _consultationRepository;
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly IRepository<Objective, Guid> _objectiveRepository;
        private readonly IRepository<Training, Guid> _trainingRepository;
        private readonly IRepository<Guest, Guid> _guestRepository;

        public DashboardAppService(
            IRepository<Patient, Guid> patientRepository,
            IRepository<Therapist, Guid> therapistRepository,
            IRepository<Clinic, Guid> clinicRepository,
            IRepository<Consultation, Guid> consultationRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Objective, Guid> objectiveRepository,
            IRepository<Training, Guid> trainingRepository,
            IRepository<Guest, Guid> guestRepository)
        {
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;
            _clinicRepository = clinicRepository;
            _consultationRepository = consultationRepository;
            _responsibleRepository = responsibleRepository;
            _objectiveRepository = objectiveRepository;
            _trainingRepository = trainingRepository;
            _guestRepository = guestRepository;
        }

        public async Task<WeCareDashboardHeaderStatsDto> GetStatsAsync()
        {
            var stats = new WeCareDashboardHeaderStatsDto();

            stats.TotalPatients = await _patientRepository.CountAsync();
            stats.TotalTherapists = await _therapistRepository.CountAsync();
            stats.TotalClinics = await _clinicRepository.CountAsync();

            if (CurrentUser.Id.HasValue)
            {
                var userId = CurrentUser.Id.Value;
                
                var therapist = await _therapistRepository.FirstOrDefaultAsync(x => x.UserId == userId);
                if (therapist != null)
                {
                    stats.MyAppointments = await _consultationRepository.CountAsync(x => x.TherapistId == therapist.Id);
                }

                var responsible = await _responsibleRepository.FirstOrDefaultAsync(x => x.UserId == userId);
                if (responsible != null)
                {
                    stats.MyPatients = await _patientRepository.CountAsync(x => x.PrincipalResponsibleId == responsible.Id);
                }
            }

            return stats;
        }

        public async Task<PatientDashboardDto> GetPatientDashboardAsync(Guid patientId)
        {
            // Security Check
            if (CurrentUser.IsInRole("Responsible"))
            {
                var responsible = await _responsibleRepository.FirstOrDefaultAsync(r => r.UserId == CurrentUser.Id);
                var patientCheck = await _patientRepository.GetAsync(patientId);
                if (responsible == null || patientCheck.PrincipalResponsibleId != responsible.Id)
                {
                    throw new Volo.Abp.UserFriendlyException("Você não tem permissão para visualizar o dashboard deste paciente.");
                }
            }
            else if (CurrentUser.IsInRole("Guest"))
            {
                var guest = await _guestRepository.FirstOrDefaultAsync(g => g.UserId == CurrentUser.Id);
                if (guest == null || guest.PatientId != patientId)
                {
                    throw new Volo.Abp.UserFriendlyException("Você não tem permissão para visualizar o dashboard deste paciente.");
                }
            }

            var patient = await _patientRepository.GetAsync(patientId);
            
            var dashboard = new PatientDashboardDto
            {
                PatientId = patient.Id,
                PatientName = patient.Name
            };

            // Objectives Stats
            var objectives = await _objectiveRepository.GetListAsync(x => x.PatientId == patientId);
            dashboard.TotalObjectives = objectives.Count;
            dashboard.ActiveObjectives = objectives.Count(x => x.Status == "Ativo");
            dashboard.CompletedObjectives = objectives.Count(x => x.Status == "Concluído" || x.Status == "Concluido");

            dashboard.ObjectivesList = objectives.Select(o => new ObjectiveStatusDto
            {
                Name = o.Name,
                Status = o.Status,
                StartDate = o.StartDate,
                ConsultationCount = 0 
            }).OrderByDescending(x => x.StartDate).Take(10).ToList();

            // Evolution / Performance History including Breakdown
            var consultationsQuery = await _consultationRepository.WithDetailsAsync(x => x.PerformedTrainings, x => x.Therapist);
            var consultations = consultationsQuery
                .Where(x => x.PatientId == patientId)
                .OrderByDescending(x => x.DateTime) 
                .ToList();

            dashboard.TotalConsultations = consultations.Count;

            // Pre-fetch training names to avoid N+1 or null issues if eager load fails
            var trainingIds = consultations
                .SelectMany(c => c.PerformedTrainings ?? new List<WeCare.PerformedTrainings.PerformedTraining>())
                .Select(pt => pt.TrainingId)
                .Distinct()
                .ToList();
            
            // Map: Id -> Name
            var trainingNames = new Dictionary<Guid, string>();
            if (trainingIds.Any())
            {
                var trainings = await _trainingRepository.GetListAsync(t => trainingIds.Contains(t.Id));
                foreach(var t in trainings)
                {
                    trainingNames[t.Id] = t.Name;
                }
            }

            // Map: ObjectiveId -> Name (for Consultations)
            var objectiveNames = objectives.ToDictionary(x => x.Id, x => x.Name);

            // 1. Detailed Consultations (Recent 20)
            foreach (var consult in consultations.Take(20))
            {
                var objName = "Geral / Sem Objetivo";
                if (consult.ObjectiveId.HasValue && consult.ObjectiveId.Value != Guid.Empty && objectiveNames.ContainsKey(consult.ObjectiveId.Value))
                {
                    objName = objectiveNames[consult.ObjectiveId.Value];
                }

                var detail = new ConsultationDetailDto
                {
                    Id = consult.Id,
                    Date = consult.DateTime,
                    TherapistName = consult.Therapist?.Name ?? "N/A",
                    ObjectiveName = objName,
                    Description = consult.Description
                };

                if (consult.PerformedTrainings != null)
                {
                    foreach (var pt in consult.PerformedTrainings)
                    {
                        var tName = trainingNames.ContainsKey(pt.TrainingId) ? trainingNames[pt.TrainingId] : "Treino não encontrado";
                        detail.PerformedTrainings.Add(new PerformedTrainingDto
                        {
                            TrainingName = tName,
                            HelpNeeded = pt.HelpNeeded.ToString(),
                            Attempts = pt.TotalAttempts,
                            Successes = pt.SuccessfulAttempts
                        });
                    }
                }
                dashboard.RecentConsultations.Add(detail);
            }
            
            // 2. Training Performance Stats (Aggregate)
            var allPerformed = consultations.SelectMany(c => c.PerformedTrainings ?? new List<WeCare.PerformedTrainings.PerformedTraining>()).ToList();
            var groupedTrainings = allPerformed.GroupBy(x => x.TrainingId);
            
            foreach (var group in groupedTrainings)
            {
                var tName = trainingNames.ContainsKey(group.Key) ? trainingNames[group.Key] : "Desconhecido";
                
                var totalExecs = group.Count();
                var totalAttempts = group.Sum(x => x.TotalAttempts);
                var totalSuccesses = group.Sum(x => x.SuccessfulAttempts);
                
                dashboard.TrainingStats.Add(new TrainingPerformanceDto
                {
                    TrainingName = tName,
                    TotalExecutions = totalExecs,
                    TotalAttempts = totalAttempts,
                    TotalSuccesses = totalSuccesses,
                    AverageSuccessRate = totalAttempts > 0 ? Math.Round(((double)totalSuccesses / totalAttempts) * 100, 1) : 0
                });
            }
            // Sort by most performed
            dashboard.TrainingStats = dashboard.TrainingStats.OrderByDescending(x => x.TotalExecutions).ToList();

            // 3. Performance History Graph (Chronological)
            var chronologicalConsults = consultations.OrderBy(x => x.DateTime).ToList();
             foreach (var consult in chronologicalConsults)
            {
                if (consult.PerformedTrainings != null && consult.PerformedTrainings.Any())
                {
                    int totalAttempts = consult.PerformedTrainings.Sum(x => x.TotalAttempts);
                    int successfulAttempts = consult.PerformedTrainings.Sum(x => x.SuccessfulAttempts);
                    
                    if (totalAttempts > 0)
                    {
                        dashboard.PerformanceHistory.Add(new PerformanceDataPointDto
                        {
                            Date = consult.DateTime,
                            Label = consult.DateTime.ToString("dd/MM"),
                            TotalAttempts = totalAttempts,
                            SuccessfulAttempts = successfulAttempts,
                            SuccessRate = Math.Round(((double)successfulAttempts / totalAttempts) * 100, 2)
                        });
                    }
                }
            }

            return dashboard;
        }
    }
}
