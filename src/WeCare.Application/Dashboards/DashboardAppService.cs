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
using WeCare.PeriodicReports;
using WeCare.Attendances;
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
        private readonly IRepository<PeriodicReport, Guid> _periodicReportRepository;
        private readonly IRepository<Attendance, Guid> _attendanceRepository;

        public DashboardAppService(
            IRepository<Patient, Guid> patientRepository,
            IRepository<Therapist, Guid> therapistRepository,
            IRepository<Clinic, Guid> clinicRepository,
            IRepository<Consultation, Guid> consultationRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Objective, Guid> objectiveRepository,
            IRepository<Training, Guid> trainingRepository,
            IRepository<Guest, Guid> guestRepository,
            IRepository<PeriodicReport, Guid> periodicReportRepository,
            IRepository<Attendance, Guid> attendanceRepository)
        {
            _patientRepository = patientRepository;
            _therapistRepository = therapistRepository;
            _clinicRepository = clinicRepository;
            _consultationRepository = consultationRepository;
            _responsibleRepository = responsibleRepository;
            _objectiveRepository = objectiveRepository;
            _trainingRepository = trainingRepository;
            _guestRepository = guestRepository;
            _periodicReportRepository = periodicReportRepository;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<WeCareDashboardHeaderStatsDto> GetStatsAsync()
        {
            var stats = new WeCareDashboardHeaderStatsDto();

            stats.TotalPatients = await _patientRepository.CountAsync();
            stats.TotalTherapists = await _therapistRepository.CountAsync();
            stats.TotalClinics = await _clinicRepository.CountAsync();

            // --- Real KPIs ---
            var now = DateTime.Now;
            var todayStart = now.Date;
            var todayEnd = todayStart.AddDays(1);
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            // Consultations today (scheduled or performed)
            stats.ConsultationsToday = await _consultationRepository.CountAsync(
                c => c.DateTime >= todayStart && c.DateTime < todayEnd);

            // Consultations this month
            stats.ConsultationsThisMonth = await _consultationRepository.CountAsync(
                c => c.DateTime >= monthStart && c.DateTime < monthEnd);

            // Active patients this month (distinct PatientIds with a consultation this month)
            var consultationsThisMonth = await _consultationRepository.GetListAsync(
                c => c.DateTime >= monthStart && c.DateTime < monthEnd);
            stats.ActivePatientsThisMonth = consultationsThisMonth
                .Select(c => c.PatientId)
                .Distinct()
                .Count();

            // Pending reports: Published but NOT yet signed by responsible
            stats.PendingReports = await _periodicReportRepository.CountAsync(
                r => r.Status == PeriodicReportStatus.Published);

            // Upcoming sessions today (next 3 from now, ordered by time)
            var upcomingQuery = await _consultationRepository.WithDetailsAsync(c => c.Patient, c => c.Therapist);
            var upcoming = upcomingQuery
                .Where(c => c.DateTime >= now && c.DateTime < todayEnd)
                .OrderBy(c => c.DateTime)
                .Take(3)
                .ToList();

            stats.UpcomingConsultationsToday = upcoming.Select(c => new UpcomingConsultationSummaryDto
            {
                Id = c.Id,
                PatientName = c.Patient?.Name ?? "Paciente",
                TherapistName = c.Therapist?.Name ?? "Terapeuta",
                DateTime = c.DateTime
            }).ToList();

            // Per-user stats
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

            // --- Monthly Attendance Stats Calculation ---
            var monthNamesPt = new string[] { "", "Jan", "Fev", "Mar", "Abr", "Mai", "Jun", "Jul", "Ago", "Set", "Out", "Nov", "Dez" };
            for (int i = 3; i >= 0; i--)
            {
                var targetMonth = now.AddMonths(-i);
                var targetMonthStart = new DateTime(targetMonth.Year, targetMonth.Month, 1);
                var targetMonthEnd = targetMonthStart.AddMonths(1);

                var attendances = await _attendanceRepository.GetListAsync(
                    a => a.SessionDate >= targetMonthStart && a.SessionDate < targetMonthEnd);

                int present = attendances.Count(a => a.Status == WeCare.Attendances.AttendanceStatus.Present);
                int absent = attendances.Count(a => a.Status == WeCare.Attendances.AttendanceStatus.Absent);
                int cancelled = attendances.Count(a => a.Status == WeCare.Attendances.AttendanceStatus.Cancelled);
                int total = present + absent + cancelled;

                double rate = total > 0 ? Math.Round(((double)present / total) * 100, 1) : 0;

                stats.MonthlyAttendanceStats.Add(new MonthlyAttendanceStatDto
                {
                    MonthName = $"{monthNamesPt[targetMonth.Month]}/{targetMonth.Year.ToString().Substring(2)}",
                    PresentCount = present,
                    AbsentCount = absent,
                    CancelledCount = cancelled,
                    PresenceRate = rate
                });
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
