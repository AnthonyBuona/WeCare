using WeCare.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

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

            var clinicsPermission = myGroup.AddPermission(WeCarePermissions.Clinics.Default, L("Permission:Clinics"));
            clinicsPermission.AddChild(WeCarePermissions.Clinics.Create, L("Permission:Clinics.Create"));
            clinicsPermission.AddChild(WeCarePermissions.Clinics.Edit, L("Permission:Clinics.Edit"));
            clinicsPermission.AddChild(WeCarePermissions.Clinics.Delete, L("Permission:Clinics.Delete"));
            clinicsPermission.AddChild(WeCarePermissions.Clinics.ManageStatus, L("Permission:Clinics.ManageStatus"));

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


        }



        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<WeCareResource>(name);
        }
    }
}