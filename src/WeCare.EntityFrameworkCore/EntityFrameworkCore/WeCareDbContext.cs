using Microsoft.EntityFrameworkCore;
using WeCare.Books;
using WeCare.Patients;
using WeCare.Responsibles;
using WeCare.Therapists;
using WeCare.Tratamentos;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace WeCare.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class WeCareDbContext :
    AbpDbContext<WeCareDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* DbSet para suas entidades */
    public DbSet<Book> Books { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Responsible> Responsibles { get; set; }
    public DbSet<Therapist> Therapists { get; set; }
    public DbSet<Tratamento> Tratamentos { get; set; }

    #region Entities from the modules

    /* * ESTA É A SEÇÃO COMPLETA QUE CORRIGE OS ERROS.
     * Note que todas as propriedades exigidas pelas interfaces estão aqui.
    */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public WeCareDbContext(DbContextOptions<WeCareDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Inclua os módulos no seu contexto de migração */
        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure suas próprias tabelas/entidades aqui */
        builder.Entity<Book>(b =>
        {
            b.ToTable(WeCareConsts.DbTablePrefix + "Books", WeCareConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
        });

        builder.Entity<Patient>(b =>
        {
            b.ToTable(WeCareConsts.DbTablePrefix + "Patients", WeCareConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);

            b.HasOne(p => p.PrincipalResponsible)
             .WithMany()
             .HasForeignKey(p => p.PrincipalResponsibleId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Responsible>(b =>
        {
            b.ToTable(WeCareConsts.DbTablePrefix + "Responsibles", WeCareConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NameResponsible).IsRequired().HasMaxLength(128);
            b.Property(x => x.EmailAddress).IsRequired();

            b.HasMany(r => r.Patients)
                .WithOne()
                .HasForeignKey(p => p.PrincipalResponsibleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Therapist>(b =>
        {
            b.ToTable(WeCareConsts.DbTablePrefix + "Therapists", WeCareConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
        });

        builder.Entity<Tratamento>(b =>
        {
            b.ToTable(WeCareConsts.DbTablePrefix + "Tratamentos", WeCareConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Tipo).IsRequired().HasMaxLength(100);

            b.HasOne(x => x.Patient)
             .WithMany(p => p.Tratamentos)
             .HasForeignKey(x => x.PatientId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade); // Se um paciente for deletado, seus tratamentos também são.

            b.HasOne(x => x.Therapist)
             .WithMany(t => t.Tratamentos)
             .HasForeignKey(x => x.TherapistId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade); // Se um terapeuta for deletado, seus tratamentos também são.
        });
    }
}