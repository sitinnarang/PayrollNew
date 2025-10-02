using Microsoft.AspNetCore.Builder;
using PayrollPro;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();

builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("PayrollPro.Web.csproj");
await builder.RunAbpModuleAsync<PayrollProWebTestModule>(applicationName: "PayrollPro.Web" );

public partial class Program
{
}
