using PayrollPro.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace PayrollPro.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(PayrollProEntityFrameworkCoreModule),
    typeof(PayrollProApplicationContractsModule)
    )]
public class PayrollProDbMigratorModule : AbpModule
{
}
