using Volo.Abp.Modularity;

namespace PayrollPro;

[DependsOn(
    typeof(PayrollProApplicationModule),
    typeof(PayrollProDomainTestModule)
)]
public class PayrollProApplicationTestModule : AbpModule
{

}
