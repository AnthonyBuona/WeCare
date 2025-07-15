using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WeCare.Application.Contracts.Consultations;
using WeCare.Patients;
using WeCare.Therapists;
using WeCare.Consultations;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form; // Adicione este using se não estiver presente

namespace WeCare.Web.Pages.RealizedConsultations
{
    public class CreateObjectiveModalModel : WeCarePageModel
    {
        [BindProperty]
        public ObjectiveViewModel Objective { get; set; }

        public SelectList TherapistLookup { get; set; }

        private readonly IConsultationAppService _consultationAppService;   
        private readonly ITherapistAppService _therapistAppService;
        private readonly IPatientAppService _patientAppService;

        public CreateObjectiveModalModel(
            IConsultationAppService consultationAppService,
            ITherapistAppService therapistAppService,
            IPatientAppService patientAppService)
        {
            _consultationAppService = consultationAppService;
            _therapistAppService = therapistAppService;
            _patientAppService = patientAppService;
            Objective = new ObjectiveViewModel();
        }

        public async Task OnGetAsync(Guid patientId)
        {
            Objective.PatientId = patientId;
            var patientDto = await _patientAppService.GetAsync(patientId);
            Objective.PatientName = patientDto.Name;

            var now = DateTime.Now;
            Objective.FirstConsultationDate = now.Date;
            Objective.FirstConsultationTime = now.ToString("HH:mm");

            var therapistLookupResult = await _therapistAppService.GetTherapistLookupAsync();
            TherapistLookup = new SelectList(therapistLookupResult.Items, "Id", "DisplayName");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (TimeSpan.TryParse(Objective.FirstConsultationTime, out var timeOfDay))
            {
                var dto = new CreateUpdateConsultationDto
                {
                    PatientId = Objective.PatientId,
                    TherapistId = Objective.TherapistId,
                    Description = Objective.ObjectiveName,
                    DateTime = Objective.FirstConsultationDate.Add(timeOfDay)
                };
                await _consultationAppService.CreateAsync(dto);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Formato de hora inválido.");
                return Page();
            }

            return NoContent();
        }
    }

}