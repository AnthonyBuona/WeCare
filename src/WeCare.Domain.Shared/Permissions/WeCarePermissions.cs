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
        public const string Default = GroupName + ".Patients";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Responsibles
    {
        public const string Default = GroupName + ".Responsibles";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Therapists
    {
        public const string Default = GroupName + ".Therapist";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Consultations
    {
        public const string Default = GroupName + ".Consultations";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Tratamentos
    {
        public const string Default = GroupName + ".Tratamentos";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Trainings
    {
        public const string Default = GroupName + ".Trainings";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Clinics
    {
        public const string Default = GroupName + ".Clinics";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ManageStatus = Default + ".ManageStatus";
        public const string Settings = GroupName + ".ClinicSettings";
    }

    public static class Objectives
    {
        public const string Default = GroupName + ".Objectives";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Guests
    {
        public const string Default = GroupName + ".Guests";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Activities
    {
        public const string Default = GroupName + ".Activities";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Attendances
    {
        public const string Default = GroupName + ".Attendances";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class PeriodicReports
    {
        public const string Default = GroupName + ".PeriodicReports";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class CrossTenantAccess
    {
        public const string Default = GroupName + ".CrossTenantAccess";
        public const string Create = Default + ".Create";
        public const string Verify = Default + ".Verify";
        public const string ViewAuditLogs = Default + ".ViewAuditLogs";
    }

    public static class Billing
    {
        public const string Default = GroupName + ".Billing";
        public const string Create = Default + ".Create";
        public const string Export = Default + ".Export";
        public const string TussMapping = Default + ".TussMapping";
    }

    public static class Gamification
    {
        public const string Default = GroupName + ".Gamification";
        public const string CreateQuest = Default + ".CreateQuest";
        public const string ExecuteQuest = Default + ".ExecuteQuest";
        public const string ViewProfile = Default + ".ViewProfile";
    }
}

