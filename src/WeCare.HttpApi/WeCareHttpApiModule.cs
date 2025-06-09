using Localization.Resources.AbpUi;
using WeCare.Localization;
using Volo.Abp.Account;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.SettingManagement;
using Volo.Abp.Localization;
using Volo.Abp.TenantManagement;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy.Localization;  // <- import adicionado

namespace WeCare;

[DependsOn(
    typeof(WeCareApplicationContractsModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpSettingManagementHttpApiModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpIdentityHttpApiModule),
    typeof(AbpTenantManagementHttpApiModule),
    typeof(AbpFeatureManagementHttpApiModule)
)]
public class WeCareHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureLocalization();
    }

    private void ConfigureLocalization()
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            // Faz o WeCareResource herdar todas as mensagens padrão do ABP UI
            options.Resources
                .Get<WeCareResource>()
                .AddBaseTypes(typeof(AbpUiResource));
        });
    }
}
