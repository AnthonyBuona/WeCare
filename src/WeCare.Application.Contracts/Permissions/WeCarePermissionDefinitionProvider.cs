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

        }



        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<WeCareResource>(name);
        }
    }
}