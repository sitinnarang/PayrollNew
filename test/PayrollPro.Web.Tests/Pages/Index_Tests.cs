using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace PayrollPro.Pages;

public class Index_Tests : PayrollProWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
