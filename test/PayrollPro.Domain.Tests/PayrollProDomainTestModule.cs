using Volo.Abp.Modularity;

namespace PayrollPro;

[DependsOn(
    typeof(PayrollProDomainModule),
    typeof(PayrollProTestBaseModule)
)]
public class PayrollProDomainTestModule : AbpModule
{

}
