using WeCare.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace WeCare.Web.Pages;

public abstract class WeCarePageModel : AbpPageModel
{
    protected WeCarePageModel()
    {
        LocalizationResourceType = typeof(WeCareResource);
    }
}
