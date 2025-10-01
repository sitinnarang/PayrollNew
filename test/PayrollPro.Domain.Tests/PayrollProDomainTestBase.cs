using Volo.Abp.Modularity;

namespace PayrollPro;

/* Inherit from this class for your domain layer tests. */
public abstract class PayrollProDomainTestBase<TStartupModule> : PayrollProTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
