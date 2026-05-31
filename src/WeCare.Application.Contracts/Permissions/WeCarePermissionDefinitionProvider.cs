using WeCare.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

using Volo.Abp.MultiTenancy;
namespace WeCare.Permissions
{
    public class WeCarePermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(WeCarePermissions.GroupName, L("Permission:WeCare"));


            var booksPermission = myGroup.AddPermission(
                WeCarePermissions.Books.Default, L("Permission:Books"));
            booksPermission.AddChild(
                WeCarePermissions.Books.Create, L("Permission:Books.Create"));
            booksPermission.AddChild(
                WeCarePermissions.Books.Edit, L("Permission:Books.Edit"));
            booksPermission.AddChild(
                WeCarePermissions.Books.Delete, L("Permission:Books.Delete"));


            var patientsPermission = myGroup.AddPermission(
                WeCarePermissions.Patients.Default, L("Permission:Patients"));
            patientsPermission.AddChild(
                WeCarePermissions.Patients.Create, L("Permission:Patients.Create"));
            patientsPermission.AddChild(
                WeCarePermissions.Patients.Edit, L("Permission:Patients.Edit"));
            patientsPermission.AddChild(
                WeCarePermissions.Patients.Delete, L("Permission:Patients.Delete"));


            var tratamentosPermission = myGroup.AddPermission(
                WeCarePermissions.Tratamentos.Default, L("Permission:Tratamentos"));
            tratamentosPermission.AddChild(
                WeCarePermissions.Tratamentos.Create, L("Permission:Tratamentos.Create"));
            tratamentosPermission.AddChild(
                WeCarePermissions.Tratamentos.Edit, L("Permission:Tratamentos.Edit"));
            tratamentosPermission.AddChild(
                WeCarePermissions.Tratamentos.Delete, L("Permission:Tratamentos.Delete"));

            var responsiblesPermission = myGroup.AddPermission(
                WeCarePermissions.Responsibles.Default, L("Permission:Responsibles"));
            responsiblesPermission.AddChild(
                WeCarePermissions.Responsibles.Create, L("Permission:Responsibles.Create"));
            responsiblesPermission.AddChild(
                WeCarePermissions.Responsibles.Edit, L("Permission:Responsibles.Edit"));
            responsiblesPermission.AddChild(
                WeCarePermissions.Responsibles.Delete, L("Permission:Responsibles.Delete"));

            var therapistsPermission = myGroup.AddPermission(
            WeCarePermissions.Therapists.Default, L("Permission:Therapists"));
            therapistsPermission.AddChild(
                WeCarePermissions.Therapists.Create, L("Permission:Therapists.Create"));
            therapistsPermission.AddChild(
                WeCarePermissions.Therapists.Edit, L("Permission:Therapists.Edit"));
            therapistsPermission.AddChild(
                WeCarePermissions.Therapists.Delete, L("Permission:Therapists.Delete"));

            var consultationsPermission = myGroup.AddPermission(
                WeCarePermissions.Consultations.Default, L("Permission:Consultations"));
            consultationsPermission.AddChild(
                WeCarePermissions.Consultations.Create, L("Permission:Consultations.Create"));
            consultationsPermission.AddChild(
                WeCarePermissions.Consultations.Edit, L("Permission:Consultations.Edit"));
            consultationsPermission.AddChild(
                WeCarePermissions.Consultations.Delete, L("Permission:Consultations.Delete"));

            var trainingsPermission = myGroup.AddPermission(WeCarePermissions.Trainings.Default, L("Permission:Trainings"));
            trainingsPermission.AddChild(WeCarePermissions.Trainings.Create, L("Permission:Trainings.Create"));
            trainingsPermission.AddChild(WeCarePermissions.Trainings.Edit, L("Permission:Trainings.Edit"));
            trainingsPermission.AddChild(WeCarePermissions.Trainings.Delete, L("Permission:Trainings.Delete"));

            var clinicsPermission = myGroup.AddPermission(WeCarePermissions.Clinics.Default, L("Permission:Clinics"), multiTenancySide: MultiTenancySides.Host);
            clinicsPermission.AddChild(WeCarePermissions.Clinics.Create, L("Permission:Clinics.Create"));
            clinicsPermission.AddChild(WeCarePermissions.Clinics.Edit, L("Permission:Clinics.Edit"));
            clinicsPermission.AddChild(WeCarePermissions.Clinics.Delete, L("Permission:Clinics.Delete"));
            clinicsPermission.AddChild(WeCarePermissions.Clinics.ManageStatus, L("Permission:Clinics.ManageStatus"));

            // Clinic Settings — available for both host and tenant admins
            myGroup.AddPermission(WeCarePermissions.Clinics.Settings, L("Permission:ClinicSettings"));

            var objectivesPermission = myGroup.AddPermission(WeCarePermissions.Objectives.Default, L("Permission:Objectives"));
            objectivesPermission.AddChild(WeCarePermissions.Objectives.Create, L("Permission:Objectives.Create"));
            objectivesPermission.AddChild(WeCarePermissions.Objectives.Edit, L("Permission:Objectives.Edit"));
            objectivesPermission.AddChild(WeCarePermissions.Objectives.Delete, L("Permission:Objectives.Delete"));

            var guestsPermission = myGroup.AddPermission(WeCarePermissions.Guests.Default, L("Permission:Guests"));
            guestsPermission.AddChild(WeCarePermissions.Guests.Create, L("Permission:Guests.Create"));
            guestsPermission.AddChild(WeCarePermissions.Guests.Edit, L("Permission:Guests.Edit"));
            guestsPermission.AddChild(WeCarePermissions.Guests.Delete, L("Permission:Guests.Delete"));

            var activitiesPermission = myGroup.AddPermission(WeCarePermissions.Activities.Default, L("Permission:Activities"));
            activitiesPermission.AddChild(WeCarePermissions.Activities.Create, L("Permission:Activities.Create"));
            activitiesPermission.AddChild(WeCarePermissions.Activities.Edit, L("Permission:Activities.Edit"));
            activitiesPermission.AddChild(WeCarePermissions.Activities.Delete, L("Permission:Activities.Delete"));

            var attendancesPermission = myGroup.AddPermission(WeCarePermissions.Attendances.Default, L("Permission:Attendances"));
            attendancesPermission.AddChild(WeCarePermissions.Attendances.Create, L("Permission:Attendances.Create"));
            attendancesPermission.AddChild(WeCarePermissions.Attendances.Edit, L("Permission:Attendances.Edit"));
            attendancesPermission.AddChild(WeCarePermissions.Attendances.Delete, L("Permission:Attendances.Delete"));

            var periodicReportsPermission = myGroup.AddPermission(WeCarePermissions.PeriodicReports.Default, L("Permission:PeriodicReports"));
            periodicReportsPermission.AddChild(WeCarePermissions.PeriodicReports.Create, L("Permission:PeriodicReports.Create"));
            periodicReportsPermission.AddChild(WeCarePermissions.PeriodicReports.Edit, L("Permission:PeriodicReports.Edit"));
            periodicReportsPermission.AddChild(WeCarePermissions.PeriodicReports.Delete, L("Permission:PeriodicReports.Delete"));

            var crossTenantPermission = myGroup.AddPermission(WeCarePermissions.CrossTenantAccess.Default, L("Permission:CrossTenantAccess"));
            crossTenantPermission.AddChild(WeCarePermissions.CrossTenantAccess.Create, L("Permission:CrossTenantAccess.Create"));
            crossTenantPermission.AddChild(WeCarePermissions.CrossTenantAccess.Verify, L("Permission:CrossTenantAccess.Verify"));
            crossTenantPermission.AddChild(WeCarePermissions.CrossTenantAccess.ViewAuditLogs, L("Permission:CrossTenantAccess.ViewAuditLogs"));

            var billingPermission = myGroup.AddPermission(WeCarePermissions.Billing.Default, L("Permission:Billing"));
            billingPermission.AddChild(WeCarePermissions.Billing.Create, L("Permission:Billing.Create"));
            billingPermission.AddChild(WeCarePermissions.Billing.Export, L("Permission:Billing.Export"));
            billingPermission.AddChild(WeCarePermissions.Billing.TussMapping, L("Permission:Billing.TussMapping"));

            var gamificationPermission = myGroup.AddPermission(WeCarePermissions.Gamification.Default, L("Permission:Gamification"));
            gamificationPermission.AddChild(WeCarePermissions.Gamification.CreateQuest, L("Permission:Gamification.CreateQuest"));
            gamificationPermission.AddChild(WeCarePermissions.Gamification.ExecuteQuest, L("Permission:Gamification.ExecuteQuest"));
            gamificationPermission.AddChild(WeCarePermissions.Gamification.ViewProfile, L("Permission:Gamification.ViewProfile"));
        }



        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<WeCareResource>(name);
        }
    }
}