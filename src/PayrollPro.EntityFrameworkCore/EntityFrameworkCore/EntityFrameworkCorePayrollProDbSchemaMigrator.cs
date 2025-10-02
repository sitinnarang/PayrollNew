using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PayrollPro.Data;
using Volo.Abp.DependencyInjection;

namespace PayrollPro.EntityFrameworkCore;

public class EntityFrameworkCorePayrollProDbSchemaMigrator
    : IPayrollProDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCorePayrollProDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the PayrollProDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<PayrollProDbContext>()
            .Database
            .MigrateAsync();
    }
}
