using System.Threading.Tasks;
using WeCare.Responsibles;
using Microsoft.AspNetCore.Mvc;

namespace WeCare.Web.Pages.Responsibles
{
    public class CreateModalModel : WeCarePageModel
    {
        [BindProperty]
        public CreateUpdateResponsibleDto Responsible { get; set; }

        private readonly IResponsibleAppService _ResponsibleAppService;

        public CreateModalModel(IResponsibleAppService ResponsibleAppService)
        {
            _ResponsibleAppService = ResponsibleAppService;
        }

        public void OnGet()
        {
            Responsible = new CreateUpdateResponsibleDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _ResponsibleAppService.CreateAsync(Responsible);
            return NoContent();
        }
    }
}