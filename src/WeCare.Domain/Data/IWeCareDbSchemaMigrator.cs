using System.Threading.Tasks;

namespace WeCare.Data;

public interface IWeCareDbSchemaMigrator
{
    Task MigrateAsync();
}
