using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using WeCare.Clinics;

namespace WeCare.Web.Themes.LeptonXLite.Components.ClinicBranding
{
    public class ClinicBrandingViewComponent : AbpViewComponent
    {
        private readonly IClinicAppService _clinicAppService;

        public ClinicBrandingViewComponent(IClinicAppService clinicAppService)
        {
            _clinicAppService = clinicAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClinicSettingsDto? settings = null;
            try
            {
                settings = await _clinicAppService.GetCurrentClinicSettingsAsync();
            }
            catch (Exception)
            {
                // Silently fail — branding is non-critical
            }

            return View("~/Pages/Shared/Components/ClinicBranding/Default.cshtml", settings);
        }
    }
}
