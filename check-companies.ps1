# Query database to see actual company IDs
cd "C:\Payrollnew\src\PayrollPro.DbMigrator"

# Use sqlite3 to query the database directly
if (Get-Command sqlite3 -ErrorAction SilentlyContinue) {
    Write-Host "Querying companies from database..."
    sqlite3 PayrollPro.db "SELECT Id, Name, Code FROM Companies ORDER BY Name;"
} else {
    # Use .NET Core to query via Entity Framework
    Write-Host "Using .NET to query companies..."
    
    # Create a temporary C# script to query the database
    $queryScript = @"
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PayrollPro.Companies;
using PayrollPro.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Modularity;

namespace TempQuery
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var application = await AbpApplicationFactory.CreateAsync<TempQueryModule>();
            await application.InitializeAsync();
            
            var context = application.ServiceProvider.GetRequiredService<PayrollProDbContext>();
            var companies = await context.Companies.OrderBy(c => c.Name).ToListAsync();
            
            Console.WriteLine("Companies in database:");
            foreach (var company in companies)
            {
                Console.WriteLine($"ID: {company.Id}, Name: {company.Name}, Code: {company.Code}");
            }
            
            await application.ShutdownAsync();
        }
    }
    
    [DependsOn(typeof(PayrollProEntityFrameworkCoreModule))]
    public class TempQueryModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.AddDbContext<PayrollProDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("Default")));
        }
    }
}
"@
    
    $queryScript | Out-File -FilePath "TempQuery.cs" -Encoding UTF8
    
    # For now, let's just check if the database file exists and its size
    $dbFile = "PayrollPro.db"
    if (Test-Path $dbFile) {
        $size = (Get-Item $dbFile).Length
        Write-Host "Database file exists. Size: $size bytes"
        
        # Try to show first few companies from seeding file
        Write-Host "`nExpected companies from seeding file:"
        Write-Host "1. TechNova Solutions (TNS001)"
        Write-Host "2. Global Innovations Inc (GII002)"
        Write-Host "3. Future Tech Corp (FTC003)"
        Write-Host "... and 7 more companies"
    } else {
        Write-Host "Database file not found!"
    }
}