using System;

namespace WeCare.CrossTenantAccess
{
    [Flags]
    public enum SharedAccessPermission
    {
        None = 0,
        Read = 1,
        Write = 2
    }
}
