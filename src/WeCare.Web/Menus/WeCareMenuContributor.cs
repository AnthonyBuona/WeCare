using System.Threading.Tasks;
using WeCare.Localization;
using WeCare.Permissions;
using WeCare.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;

namespace WeCare.Web.Menus;

public class WeCareMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<WeCareResource>();

        // Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                WeCareMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );
        
        // Scheduling (Calendar)
        context.Menu.AddItem(
            new ApplicationMenuItem(
                WeCareMenus.Scheduling,
                l["Menu:Scheduling"],
                url: "/Calendar",
                icon: "fa fa-calendar-check-o",
                order: 2
            ).RequirePermissions(WeCarePermissions.Consultations.Default)
        );

        // Novo menu para Terapeutas
        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Therapists",
                "Terapeutas",
                url: "/Therapists",
                icon: "fa fa-user-md"
            ).RequirePermissions(WeCarePermissions.Therapists.Default)
        );

        // Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 6;
        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 1);

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }

        administration.AddItem(new ApplicationMenuItem(
            "WeCare.Clinics",
            l["Menu:Clinics"],
            url: "/Clinics",
            icon: "fa fa-hospital-o",
            order: 2
        ).RequirePermissions(WeCarePermissions.Clinics.Default));

        // Configurações da Clínica (accessible to both host and tenant admins)
        administration.AddItem(new ApplicationMenuItem(
            "WeCare.Clinics.Settings",
            "Configurações",
            url: "/Clinics/Settings",
            icon: "fa fa-cog",
            order: 3
        ).RequirePermissions(WeCarePermissions.Clinics.Settings));

        // "Config Global" — only for host admin, remove for tenant admins
        var currentTenant = context.ServiceProvider.GetRequiredService<ICurrentTenant>();
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 4);
        
        if (currentTenant.Id != null)
        {
            // Tenant admin — remove Config Global (ABP settings)
            var settingItem = administration.GetMenuItemOrNull(SettingManagementMenuNames.GroupName);
            if (settingItem != null)
            {
                administration.Items.Remove(settingItem);
            }
        }
        else
        {
            // Host admin — rename to "Config Global"
            var settingItem = administration.GetMenuItemOrNull(SettingManagementMenuNames.GroupName);
            if (settingItem != null)
            {
                settingItem.DisplayName = "Config Global";
            }
        }

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Pacientes",
                l["Menu:Patients"],
                icon: "fa fa-user"
            ).RequirePermissions(WeCarePermissions.Patients.Default)
            // Novo subitem: lista de pacientes
            .AddItem(
                new ApplicationMenuItem(
                    "Pacientes.Patients",
                    l["Menu:Patients"],  // mesmo rótulo “Patients”
                    url: "/Patients"
                ).RequirePermissions(WeCarePermissions.Patients.Default)
            )
            // Subitem existente: responsáveis
            .AddItem(
                new ApplicationMenuItem(
                    "Pacientes.Responsaveis",
                    l["Menu:Patients.Responsibles"],
                    url: "/Responsibles"
                ).RequirePermissions(WeCarePermissions.Responsibles.Default)
            )
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Tratamento",
                l["Tratamento"],
                icon: "fa fa-calendar"
            ).RequirePermissions(WeCarePermissions.Patients.Default)
            // Novo subitem: lista de pacientes
            .AddItem(
                new ApplicationMenuItem(
                    "Tratamento.Consultas",
                    l["Consultas"], 
                    url: "/Consultas"
                ).RequirePermissions(WeCarePermissions.Consultations.Default)
            )
            .AddItem(
                new ApplicationMenuItem(
                    "Tratamento.Tratamentos",
                    l["Menu:Tratamentos"], 
                    url: "/Tratamentos"
                ).RequirePermissions(WeCarePermissions.Tratamentos.Default)
            )

        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Objectives",
                l["Menu:Objectives"],
                url: "/Objectives",
                icon: "fa fa-bullseye"
            ).RequirePermissions(WeCarePermissions.Objectives.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Guests",
                l["Menu:Guests"],
                url: "/Guests",
                icon: "fa fa-users"
            ).RequirePermissions(WeCarePermissions.Guests.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                "Activities",
                l["Menu:Activities"],
                url: "/Activities",
                icon: "fa fa-tasks"
            ).RequirePermissions(WeCarePermissions.Activities.Default)
        );

        return Task.CompletedTask;
    }
}
