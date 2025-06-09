using WeCare.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace WeCare.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WeCareEntityFrameworkCoreModule),
    typeof(WeCareApplicationContractsModule)
)]
public class WeCareDbMigratorModule : AbpModule
{
}
