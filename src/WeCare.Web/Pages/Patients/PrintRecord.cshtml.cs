using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Repositories;
using WeCare.Patients;
using WeCare.Responsibles;
using WeCare.Consultations;
using WeCare.Objectives;
using WeCare.Trainings;
using WeCare.Attendances;
using WeCare.PeriodicReports;
using WeCare.Therapists;

namespace WeCare.Web.Pages.Patients
{
    public class PrintRecordModel : WeCarePageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public Patient Patient { get; set; } = null!;
        public Responsible? PrincipalResponsible { get; set; }
        public List<Consultation> Consultations { get; set; } = new();
        public List<Objective> Objectives { get; set; } = new();
        public List<Training> Trainings { get; set; } = new();
        public List<PeriodicReport> PeriodicReports { get; set; } = new();
        public List<Attendance> Attendances { get; set; } = new();
        public Dictionary<Guid, string> TherapistNames { get; set; } = new();

        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Responsible, Guid> _responsibleRepository;
        private readonly IRepository<Consultation, Guid> _consultationRepository;
        private readonly IRepository<Objective, Guid> _objectiveRepository;
        private readonly IRepository<Training, Guid> _trainingRepository;
        private readonly IRepository<Attendance, Guid> _attendanceRepository;
        private readonly IRepository<PeriodicReport, Guid> _periodicReportRepository;
        private readonly IRepository<Therapist, Guid> _therapistRepository;

        public PrintRecordModel(
            IRepository<Patient, Guid> patientRepository,
            IRepository<Responsible, Guid> responsibleRepository,
            IRepository<Consultation, Guid> consultationRepository,
            IRepository<Objective, Guid> objectiveRepository,
            IRepository<Training, Guid> trainingRepository,
            IRepository<Attendance, Guid> attendanceRepository,
            IRepository<PeriodicReport, Guid> periodicReportRepository,
            IRepository<Therapist, Guid> therapistRepository)
        {
            _patientRepository = patientRepository;
            _responsibleRepository = responsibleRepository;
            _consultationRepository = consultationRepository;
            _objectiveRepository = objectiveRepository;
            _trainingRepository = trainingRepository;
            _attendanceRepository = attendanceRepository;
            _periodicReportRepository = periodicReportRepository;
            _therapistRepository = therapistRepository;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id == Guid.Empty)
            {
                return NotFound();
            }

            var patient = await _patientRepository.FindAsync(Id);
            if (patient == null)
            {
                return NotFound();
            }
            Patient = patient;

            if (Patient.PrincipalResponsibleId != Guid.Empty)
            {
                PrincipalResponsible = await _responsibleRepository.FindAsync(Patient.PrincipalResponsibleId);
            }

            // Realized Consultations (evolution history)
            var consultationsQuery = await _consultationRepository.WithDetailsAsync(c => c.Therapist);
            Consultations = consultationsQuery
                .Where(c => c.PatientId == Id && c.Status == ConsultationStatus.Realizada)
                .OrderBy(c => c.DateTime)
                .ToList();

            // Objectives and associated Trainings
            Objectives = await _objectiveRepository.GetListAsync(o => o.PatientId == Id);
            var objectiveIds = Objectives.Select(o => o.Id).ToList();
            if (objectiveIds.Any())
            {
                Trainings = await _trainingRepository.GetListAsync(t => objectiveIds.Contains(t.ObjectiveId));
            }

            // Periodic Reports that are signed by the responsible
            PeriodicReports = await _periodicReportRepository.GetListAsync(
                r => r.PatientId == Id && r.Status == PeriodicReportStatus.SignedByResponsible);

            // Fetch therapist names to show who signed/wrote the reports
            var therapistIdsInReports = PeriodicReports.Select(r => r.TherapistId).Distinct().ToList();
            if (therapistIdsInReports.Any())
            {
                var therapists = await _therapistRepository.GetListAsync(t => therapistIdsInReports.Contains(t.Id));
                TherapistNames = therapists.ToDictionary(t => t.Id, t => t.Name);
            }

            // Attendances (frequencies)
            Attendances = await _attendanceRepository.GetListAsync(a => a.PatientId == Id);
            Attendances = Attendances.OrderBy(a => a.SessionDate).ToList();

            return Page();
        }
    }
}
