using WeCare.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace WeCare.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WeCareController : AbpControllerBase
{
    protected WeCareController()
    {
        LocalizationResource = typeof(WeCareResource);
    }
}
