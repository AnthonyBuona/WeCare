using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.ObjectMapping;
using WeCare.Application.Contracts.Consultations;
using WeCare.Patients;
using WeCare.Objectives;

namespace WeCare.Web.Pages.RealizedConsultations
{
    public class IndexModel : WeCarePageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid PatientId { get; set; } // Alterado para não ser nulo, garantindo que sempre teremos o ID
        public ObjectiveGroupViewModel ObjectiveGroups { get; set; }

        public string PatientName { get; set; }

        private readonly IPatientAppService _patientAppService;
        private readonly IConsultationAppService _consultationAppService;
        private readonly IObjectiveAppService _objectiveAppService;

        public IndexModel(
            IPatientAppService patientAppService,
            IObjectiveAppService objectiveAppService,
            IConsultationAppService consultationAppService)
        {
            _patientAppService = patientAppService;
            _objectiveAppService = objectiveAppService;
            _consultationAppService = consultationAppService;
            ObjectiveGroups = new ObjectiveGroupViewModel();
        }

        // OnGet agora apenas prepara a página principal com o nome do paciente
        public async Task OnGetAsync()
        {
            var patient = await _patientAppService.GetAsync(PatientId);
            PatientName = patient.Name;
        }

        // Novo handler que retorna a lista de objetivos como uma PartialView
        public async Task<IActionResult> OnGetObjectiveListAsync(Guid patientId)
        {
            var objectiveGroups = await _objectiveAppService.GetGroupedObjectivesByPatientAsync(patientId);
            var viewModel = new ConsultationHistoryViewModel();

            foreach (var group in objectiveGroups)
            {
                var objectiveViewModel = new ObjectiveDisplayViewModel
                {
                    Name = group.ObjectiveName,
                    Progress = Math.Min(100, group.Consultations.Count * 10), // Lógica de progresso estável
                    Consultations = ObjectMapper.Map<List<ConsultationInGroupDto>, List<ConsultationItemViewModel>>(group.Consultations)
                };
                viewModel.Objectives.Add(objectiveViewModel);
            }

            return Partial("_ObjectiveList", viewModel);
        }
    }
}