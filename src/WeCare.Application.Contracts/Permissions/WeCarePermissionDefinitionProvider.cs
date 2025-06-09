using WeCare.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace WeCare.Permissions;

public class WeCarePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WeCarePermissions.GroupName);

        // --- Books permissions ---
        var booksPermission = myGroup.AddPermission(
            WeCarePermissions.Books.Default,
            L("Permission:Books")
        );
        booksPermission.AddChild(
            WeCarePermissions.Books.Create,
            L("Permission:Books.Create")
        );
        booksPermission.AddChild(
            WeCarePermissions.Books.Edit,
            L("Permission:Books.Edit")
        );
        booksPermission.AddChild(
            WeCarePermissions.Books.Delete,
            L("Permission:Books.Delete")
        );

        // --- Patients permissions (novo) ---
        var patientsPermission = myGroup.AddPermission(
            WeCarePermissions.Patients.Default,
            L("Permission:Patients")
        );
        patientsPermission.AddChild(
            WeCarePermissions.Patients.Create,
            L("Permission:Patients.Create")
        );
        patientsPermission.AddChild(
            WeCarePermissions.Patients.Edit,
            L("Permission:Patients.Edit")
        );
        patientsPermission.AddChild(
            WeCarePermissions.Patients.Delete,
            L("Permission:Patients.Delete")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WeCareResource>(name);
    }
}
