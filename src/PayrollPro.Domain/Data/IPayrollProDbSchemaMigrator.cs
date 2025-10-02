using System.Threading.Tasks;

namespace PayrollPro.Data;

public interface IPayrollProDbSchemaMigrator
{
    Task MigrateAsync();
}
