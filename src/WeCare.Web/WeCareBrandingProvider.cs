using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using WeCare.Localization;

namespace WeCare.Web;

[Dependency(ReplaceServices = true)]
public class WeCareBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<WeCareResource> _localizer;

    public WeCareBrandingProvider(IStringLocalizer<WeCareResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
