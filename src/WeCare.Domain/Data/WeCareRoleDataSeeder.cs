using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using WeCare.Permissions;

namespace WeCare.Data
{
    public class WeCareRoleDataSeeder : IDataSeedContributor, ITransientDependency
    {
        private readonly IdentityRoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;

        public WeCareRoleDataSeeder(
            IdentityRoleManager roleManager,
            IPermissionManager permissionManager)
        {
            _roleManager = roleManager;
            _permissionManager = permissionManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            await CreateRolesAsync(context.TenantId);
            
            // Atribuir permissões baseadas nas Personas definidas

            // 1. ClinicAdmin (Dono da Clínica)
            await GrantPermissionsAsync("ClinicAdmin", new[] {
                WeCarePermissions.Patients.Default,
                WeCarePermissions.Patients.Create,
                WeCarePermissions.Patients.Edit,
                WeCarePermissions.Patients.Delete,

                WeCarePermissions.Therapists.Default,
                WeCarePermissions.Therapists.Create,
                WeCarePermissions.Therapists.Edit,
                WeCarePermissions.Therapists.Delete,

                WeCarePermissions.Consultations.Default,
                WeCarePermissions.Consultations.Create,
                WeCarePermissions.Consultations.Edit,
                WeCarePermissions.Consultations.Delete,

                WeCarePermissions.Objectives.Default,
                WeCarePermissions.Objectives.Create,
                WeCarePermissions.Objectives.Edit,
                WeCarePermissions.Objectives.Delete,
                
                WeCarePermissions.Trainings.Default,
                WeCarePermissions.Trainings.Create,
                WeCarePermissions.Trainings.Edit,
                WeCarePermissions.Trainings.Delete,

                WeCarePermissions.Tratamentos.Default,
                WeCarePermissions.Tratamentos.Create,
                WeCarePermissions.Tratamentos.Edit,
                WeCarePermissions.Tratamentos.Delete,

                WeCarePermissions.Activities.Default,
                WeCarePermissions.Activities.Create,
                WeCarePermissions.Activities.Edit,
                WeCarePermissions.Activities.Delete,
                
                WeCarePermissions.Guests.Default,
                WeCarePermissions.Guests.Create,
                WeCarePermissions.Guests.Edit,
                WeCarePermissions.Guests.Delete,

                WeCarePermissions.Books.Default // Exemplo legado
            }, context.TenantId);

            // 2. Therapist (Terapeuta)
            await GrantPermissionsAsync("Therapist", new[] {
                WeCarePermissions.Patients.Default,
                WeCarePermissions.Patients.Create,
                WeCarePermissions.Patients.Edit,
                WeCarePermissions.Patients.Delete, 

                WeCarePermissions.Consultations.Default,
                WeCarePermissions.Consultations.Create,
                WeCarePermissions.Consultations.Edit,
                WeCarePermissions.Consultations.Delete,

                WeCarePermissions.Objectives.Default,
                WeCarePermissions.Objectives.Create,
                WeCarePermissions.Objectives.Edit,
                WeCarePermissions.Objectives.Delete,
                
                WeCarePermissions.Trainings.Default,
                WeCarePermissions.Trainings.Create,
                WeCarePermissions.Trainings.Edit,
                WeCarePermissions.Trainings.Delete,

                WeCarePermissions.Tratamentos.Default,
                WeCarePermissions.Tratamentos.Create,
                WeCarePermissions.Tratamentos.Edit,
                WeCarePermissions.Tratamentos.Delete,

                WeCarePermissions.Activities.Default // Adicionado acesso a Atividades
            }, context.TenantId);

            // 3. Responsible (Responsável)
            await GrantPermissionsAsync("Responsible", new[] {
                 WeCarePermissions.Patients.Default,
                 WeCarePermissions.Consultations.Default,
                 WeCarePermissions.Tratamentos.Default,
                 WeCarePermissions.Trainings.Default,
                 WeCarePermissions.Activities.Default,
                 WeCarePermissions.Objectives.Default,
                 WeCarePermissions.Guests.Default,
            }, context.TenantId);
            
            // 4. Guest (Convidado)
            // Acesso somente leitura
            await GrantPermissionsAsync("Guest", new[] {
                WeCarePermissions.Patients.Default,
                WeCarePermissions.Consultations.Default,
                WeCarePermissions.Tratamentos.Default,
                WeCarePermissions.Trainings.Default,
                WeCarePermissions.Activities.Default,
                WeCarePermissions.Objectives.Default
            }, context.TenantId);
        }

        private async Task CreateRolesAsync(System.Guid? tenantId)
        {
            await CreateRoleIfNotExistsAsync("ClinicAdmin", tenantId);
            await CreateRoleIfNotExistsAsync("Therapist", tenantId);
            await CreateRoleIfNotExistsAsync("Responsible", tenantId);
            await CreateRoleIfNotExistsAsync("Guest", tenantId);
        }

        private async Task CreateRoleIfNotExistsAsync(string roleName, System.Guid? tenantId)
        {
            if (await _roleManager.FindByNameAsync(roleName) == null)
            {
                await _roleManager.CreateAsync(new IdentityRole(System.Guid.NewGuid(), roleName, tenantId));
            }
        }

        private async Task GrantPermissionsAsync(string roleName, string[] permissions, System.Guid? tenantId)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                foreach (var permission in permissions)
                {
                    await _permissionManager.SetForRoleAsync(roleName, permission, true);
                }
            }
        }
    }
}
