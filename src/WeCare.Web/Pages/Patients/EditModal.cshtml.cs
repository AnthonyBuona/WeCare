using System;
using System.Threading.Tasks;
using WeCare.Patients;
using Microsoft.AspNetCore.Mvc;

namespace WeCare.Web.Pages.Patients;

public class EditModalModel : WeCarePageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdatePatientDto Patient { get; set; }

    private readonly IPatientAppService _PatientAppService;

    public EditModalModel(IPatientAppService PatientAppService)
    {
        _PatientAppService = PatientAppService;
    }

    public async Task OnGetAsync()
    {
        var PatientDto = await _PatientAppService.GetAsync(Id);
        Patient = ObjectMapper.Map<PatientDto, CreateUpdatePatientDto>(PatientDto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _PatientAppService.UpdateAsync(Id, Patient);
        return NoContent();
    }
}