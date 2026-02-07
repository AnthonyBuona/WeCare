using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WeCare.EntityFrameworkCore;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands) */
public class WeCareDbContextFactory : IDesignTimeDbContextFactory<WeCareDbContext>
{
    public WeCareDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        
        WeCareEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<WeCareDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));
        
        return new WeCareDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../WeCare.DbMigrator/"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.secrets.json", optional: true);

        return builder.Build();
    }
}
