using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WeCare.Web.Pages.Calendar
{
    public class IndexModel : WeCarePageModel
    {
        public async Task OnGetAsync()
        {
            await Task.CompletedTask;
        }
    }
}
