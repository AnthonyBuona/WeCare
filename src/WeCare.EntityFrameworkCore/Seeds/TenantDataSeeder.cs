using System;
using System.Linq;                         
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;       
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using WeCare.EntityFrameworkCore;
using Volo.Abp.Identity;              
public class TenantDataSeeder : IDataSeedContributor, ITransientDependency
{
    private readonly WeCareDbContext _dbContext;

    public TenantDataSeeder(WeCareDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId == null)
            return;

        // Se não existir nenhuma role para este tenant…
        if (!await _dbContext.Roles.AnyAsync(r => r.TenantId == context.TenantId))
        {
            // Cria uma role "Padrão" atrelada a este tenant
            var padrao = new IdentityRole(
                context.TenantId.Value,
                "Padrão"
            );

            _dbContext.Roles.Add(padrao);
            await _dbContext.SaveChangesAsync();
        }
    }
}
