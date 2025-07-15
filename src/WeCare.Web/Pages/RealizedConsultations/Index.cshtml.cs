using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.Application.Contracts.Consultations;
using WeCare.Patients;

namespace WeCare.Web.Pages.RealizedConsultations
{
    public class IndexModel : WeCarePageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid? PatientId { get; set; }

        public string PatientName { get; set; }

        // Alterado para usar a nova ViewModel principal
        public ConsultationHistoryViewModel ViewModel { get; set; }

        private readonly IPatientAppService _patientAppService;
        private readonly IConsultationAppService _consultationAppService;

        public IndexModel(
            IPatientAppService patientAppService,
            IConsultationAppService consultationAppService)
        {
            _patientAppService = patientAppService;
            _consultationAppService = consultationAppService;
            // Instancia a nova ViewModel principal
            ViewModel = new ConsultationHistoryViewModel();
        }

        public async Task OnGetAsync()
        {
            if (PatientId.HasValue)
            {
                var patient = await _patientAppService.GetAsync(PatientId.Value);
                PatientName = patient.Name;

                var objectiveGroups = await _consultationAppService.GetGroupedByPatientAsync(PatientId.Value);

                foreach (var group in objectiveGroups)
                {
                    // Usa a nova 'ObjectiveDisplayViewModel'
                    var objectiveViewModel = new ObjectiveDisplayViewModel
                    {
                        Name = group.ObjectiveName,
                        Progress = new Random().Next(30, 90),
                        Consultations = new System.Collections.Generic.List<ConsultationItemViewModel>()
                    };

                    foreach (var c in group.Consultations)
                    {
                        objectiveViewModel.Consultations.Add(new ConsultationItemViewModel
                        {
                            TherapistName = c.TherapistName,
                            TherapistSpecialization = c.TherapistSpecialization,
                            Description = c.Description,
                            DateTime = c.DateTime
                        });
                    }
                    ViewModel.Objectives.Add(objectiveViewModel);
                }
            }
        }
    }
}