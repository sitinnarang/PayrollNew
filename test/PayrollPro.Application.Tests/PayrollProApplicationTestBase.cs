using Volo.Abp.Modularity;

namespace PayrollPro;

public abstract class PayrollProApplicationTestBase<TStartupModule> : PayrollProTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
