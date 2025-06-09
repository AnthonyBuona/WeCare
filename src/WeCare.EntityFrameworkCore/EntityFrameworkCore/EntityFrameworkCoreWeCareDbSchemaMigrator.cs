using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeCare.Data;
using Volo.Abp.DependencyInjection;

namespace WeCare.EntityFrameworkCore;

public class EntityFrameworkCoreWeCareDbSchemaMigrator
    : IWeCareDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreWeCareDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the WeCareDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<WeCareDbContext>()
            .Database
            .MigrateAsync();
    }
}
