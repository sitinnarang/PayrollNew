using PayrollPro.Samples;
using Xunit;

namespace PayrollPro.EntityFrameworkCore.Domains;

[Collection(PayrollProTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<PayrollProEntityFrameworkCoreTestModule>
{

}
