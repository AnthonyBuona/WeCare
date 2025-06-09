using WeCare.Localization;
using Volo.Abp.Application.Services;

namespace WeCare;

/* Inherit your application services from this class.
 */
public abstract class WeCareAppService : ApplicationService
{
    protected WeCareAppService()
    {
        LocalizationResource = typeof(WeCareResource);
    }
}
