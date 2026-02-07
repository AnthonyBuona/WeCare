using System.Threading.Tasks;
using WeCare.Localization;
using WeCare.Permissions;
using WeCare.MultiTenancy;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.UI.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;

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

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
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
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 7);

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
                ).RequirePermissions(WeCarePermissions.Patients.Default)
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
