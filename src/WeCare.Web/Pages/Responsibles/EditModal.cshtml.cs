using System;
using System.Threading.Tasks;
using WeCare.Responsibles;
using Microsoft.AspNetCore.Mvc;

namespace WeCare.Web.Pages.Responsibles;

public class EditModalModel : WeCarePageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public CreateUpdateResponsibleDto Responsible { get; set; }

    private readonly IResponsibleAppService _ResponsibleAppService;

    public EditModalModel(IResponsibleAppService ResponsibleAppService)
    {
        _ResponsibleAppService = ResponsibleAppService;
    }

    public async Task OnGetAsync()
    {
        var ResponsibleDto = await _ResponsibleAppService.GetAsync(Id);
        Responsible = ObjectMapper.Map<ResponsibleDto, CreateUpdateResponsibleDto>(ResponsibleDto);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _ResponsibleAppService.UpdateAsync(Id, Responsible);
        return NoContent();
    }
}