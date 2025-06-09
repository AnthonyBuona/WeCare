namespace WeCare.Permissions;

public static class WeCarePermissions
{
    public const string GroupName = "WeCare";


    public static class Books
    {
        public const string Default = GroupName + ".Books";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Patients
    {
        public const string Default = GroupName + ".";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Responsibles
    {
        public const string Default = GroupName + ".";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }


    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
}
