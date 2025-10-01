using PayrollPro.Samples;
using Xunit;

namespace PayrollPro.EntityFrameworkCore.Applications;

[Collection(PayrollProTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<PayrollProEntityFrameworkCoreTestModule>
{

}
