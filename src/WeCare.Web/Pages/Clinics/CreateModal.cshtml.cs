using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeCare.Clinics;

namespace WeCare.Web.Pages.Clinics;

public class CreateModalModel : WeCarePageModel
{
    [BindProperty]
    public CreateClinicInput Input { get; set; }

    private readonly IClinicManagementAppService _clinicManagementAppService;

    public CreateModalModel(IClinicManagementAppService clinicManagementAppService)
    {
        _clinicManagementAppService = clinicManagementAppService;
    }

    public void OnGet()
    {
        Input = new CreateClinicInput();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _clinicManagementAppService.CreateAsync(Input);
        return NoContent();
    }
}
