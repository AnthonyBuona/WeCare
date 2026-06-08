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

        // ----------------------------------------------------
        // Category 1: Painel Principal (Dashboard & Scheduling)
        // ----------------------------------------------------
        var mainPanel = new ApplicationMenuItem(
            "WeCare.MainPanel",
            l["Menu:MainPanel"],
            icon: "fa fa-dashboard",
            order: 1
        );

        mainPanel.AddItem(new ApplicationMenuItem(
            WeCareMenus.Home,
            l["Menu:Home"],
            url: "~/",
            icon: "fa fa-home"
        ));

        mainPanel.AddItem(new ApplicationMenuItem(
            WeCareMenus.Scheduling,
            l["Menu:Scheduling"],
            url: "/Calendar",
            icon: "fa fa-calendar-check-o"
        ).RequirePermissions(WeCarePermissions.Consultations.Default));

        context.Menu.AddItem(mainPanel);

        // ----------------------------------------------------
        // Category 2: Acompanhamento Clínico (Clinical Care)
        // ----------------------------------------------------
        var clinicalCare = new ApplicationMenuItem(
            "WeCare.ClinicalCare",
            l["Menu:ClinicalCare"],
            icon: "fa fa-heartbeat",
            order: 2
        ).RequirePermissions(WeCarePermissions.Patients.Default); // broad default check

        clinicalCare.AddItem(new ApplicationMenuItem(
            WeCareMenus.Attendances,
            l["Menu:Attendances"],
            url: "/Attendances",
            icon: "fa fa-check-square-o"
        ).RequirePermissions(WeCarePermissions.Attendances.Default));

        clinicalCare.AddItem(new ApplicationMenuItem(
            "Objectives",
            l["Menu:Objectives"],
            url: "/Objectives",
            icon: "fa fa-bullseye"
        ).RequirePermissions(WeCarePermissions.Objectives.Default));

        clinicalCare.AddItem(new ApplicationMenuItem(
            "Tratamento.Consultas",
            l["Consultas"],
            url: "/Consultas",
            icon: "fa fa-stethoscope"
        ).RequirePermissions(WeCarePermissions.Consultations.Default));

        clinicalCare.AddItem(new ApplicationMenuItem(
            "Tratamento.Tratamentos",
            l["Menu:Tratamentos"],
            url: "/Tratamentos",
            icon: "fa fa-calendar"
        ).RequirePermissions(WeCarePermissions.Tratamentos.Default));

        clinicalCare.AddItem(new ApplicationMenuItem(
            WeCareMenus.PeriodicReports,
            l["Menu:PeriodicReports"],
            url: "/PeriodicReports",
            icon: "fa fa-file-text-o"
        ).RequirePermissions(WeCarePermissions.PeriodicReports.Default));

        clinicalCare.AddItem(new ApplicationMenuItem(
            "WeCare.CrossTenantAccess.Timeline",
            "Prontuário Compartilhado (Terapeuta)",
            url: "/CrossTenantAccess/Timeline",
            icon: "fa fa-share-alt"
        ).RequirePermissions(WeCarePermissions.CrossTenantAccess.Default));

        clinicalCare.AddItem(new ApplicationMenuItem(
            WeCareMenus.Gamification,
            l["Menu:Gamification"],
            url: "/Gamification",
            icon: "fa fa-gamepad"
        ).RequirePermissions(WeCarePermissions.Gamification.Default));

        context.Menu.AddItem(clinicalCare);

        // ----------------------------------------------------
        // Category 3: Cadastros & Equipe (Directories & Staff)
        // ----------------------------------------------------
        var directories = new ApplicationMenuItem(
            "WeCare.Directories",
            l["Menu:Directories"],
            icon: "fa fa-folder-open",
            order: 3
        ).RequirePermissions(WeCarePermissions.Patients.Default);

        directories.AddItem(new ApplicationMenuItem(
            "Pacientes.Patients",
            l["Menu:Patients"],
            url: "/Patients",
            icon: "fa fa-user"
        ).RequirePermissions(WeCarePermissions.Patients.Default));

        directories.AddItem(new ApplicationMenuItem(
            "Pacientes.Responsaveis",
            l["Menu:Patients.Responsibles"],
            url: "/Responsibles",
            icon: "fa fa-users"
        ).RequirePermissions(WeCarePermissions.Responsibles.Default));

        directories.AddItem(new ApplicationMenuItem(
            "Therapists",
            l["Menu:Therapists"],
            url: "/Therapists",
            icon: "fa fa-user-md"
        ).RequirePermissions(WeCarePermissions.Therapists.Default));

        directories.AddItem(new ApplicationMenuItem(
            "Guests",
            l["Menu:Guests"],
            url: "/Guests",
            icon: "fa fa-handshake-o"
        ).RequirePermissions(WeCarePermissions.Guests.Default));

        directories.AddItem(new ApplicationMenuItem(
            "Activities",
            l["Menu:Activities"],
            url: "/Activities",
            icon: "fa fa-tasks"
        ).RequirePermissions(WeCarePermissions.Activities.Default));

        directories.AddItem(new ApplicationMenuItem(
            "Trainings",
            l["Menu:Trainings"],
            url: "/Trainings",
            icon: "fa fa-heartbeat"
        ).RequirePermissions(WeCarePermissions.Trainings.Default));

        context.Menu.AddItem(directories);

        // ----------------------------------------------------
        // Category 4: Faturamento & Utilidades (Billing & Utilities)
        // ----------------------------------------------------
        var billingAndRPG = new ApplicationMenuItem(
            "WeCare.BillingAndRPG",
            l["Menu:BillingAndRPG"],
            icon: "fa fa-credit-card",
            order: 4
        ).RequirePermissions(WeCarePermissions.Billing.Default); // default faturamento check

        billingAndRPG.AddItem(new ApplicationMenuItem(
            WeCareMenus.Billing,
            l["Menu:Billing"],
            url: "/Billing",
            icon: "fa fa-file-excel-o"
        ).RequirePermissions(WeCarePermissions.Billing.Default));

        billingAndRPG.AddItem(new ApplicationMenuItem(
            "WeCare.CrossTenantAccess.Consent",
            "Meu Consentimento (Responsável)",
            url: "/CrossTenantAccess",
            icon: "fa fa-key"
        ).RequirePermissions(WeCarePermissions.CrossTenantAccess.Default));

        context.Menu.AddItem(billingAndRPG);

        // ----------------------------------------------------
        // Category 5: Administration (ABP default Identity & Clinic management)
        // ----------------------------------------------------
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

        administration.AddItem(new ApplicationMenuItem(
            "WeCare.Clinics.Settings",
            l["Settings"],
            url: "/Clinics/Settings",
            icon: "fa fa-cog",
            order: 3
        ).RequirePermissions(WeCarePermissions.Clinics.Settings));

        var currentTenant = context.ServiceProvider.GetRequiredService<ICurrentTenant>();
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 4);
        
        if (currentTenant.Id != null)
        {
            var settingItem = administration.GetMenuItemOrNull(SettingManagementMenuNames.GroupName);
            if (settingItem != null)
            {
                administration.Items.Remove(settingItem);
            }
        }
        else
        {
            var settingItem = administration.GetMenuItemOrNull(SettingManagementMenuNames.GroupName);
            if (settingItem != null)
            {
                settingItem.DisplayName = "Config Global";
            }
        }

        return Task.CompletedTask;
    }
}
