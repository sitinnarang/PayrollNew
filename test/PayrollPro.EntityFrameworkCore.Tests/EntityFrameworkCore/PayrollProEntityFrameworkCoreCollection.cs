using Xunit;

namespace PayrollPro.EntityFrameworkCore;

[CollectionDefinition(PayrollProTestConsts.CollectionDefinitionName)]
public class PayrollProEntityFrameworkCoreCollection : ICollectionFixture<PayrollProEntityFrameworkCoreFixture>
{

}
