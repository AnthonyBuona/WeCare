using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace WeCare.Data;

/* This is used if database provider does't define
 * IWeCareDbSchemaMigrator implementation.
 */
public class NullWeCareDbSchemaMigrator : IWeCareDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
