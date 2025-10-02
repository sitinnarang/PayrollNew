using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PayrollPro.Data;

/* This is used if database provider does't define
 * IPayrollProDbSchemaMigrator implementation.
 */
public class NullPayrollProDbSchemaMigrator : IPayrollProDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
