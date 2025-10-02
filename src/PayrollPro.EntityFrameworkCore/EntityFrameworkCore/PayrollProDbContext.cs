using Microsoft.EntityFrameworkCore;
using PayrollPro.Companies;
using PayrollPro.Employees;
using PayrollPro.Payrolls;
using PayrollPro.Timesheets;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace PayrollPro.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class PayrollProDbContext :
    AbpDbContext<PayrollProDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    
    // Payroll System Entities
    public DbSet<Company> Companies { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<PayrollRecord> PayrollRecords { get; set; }
    public DbSet<Timesheet> Timesheets { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public PayrollProDbContext(DbContextOptions<PayrollProDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        // Configure Company entity
        builder.Entity<Company>(b =>
        {
            b.ToTable(PayrollProConsts.DbTablePrefix + "Companies", PayrollProConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Code).HasMaxLength(100);
            b.Property(x => x.Description).HasMaxLength(500);
            b.Property(x => x.Address).IsRequired().HasMaxLength(500);
            b.Property(x => x.City).HasMaxLength(100);
            b.Property(x => x.State).HasMaxLength(100);
            b.Property(x => x.ZipCode).HasMaxLength(20);
            b.Property(x => x.Country).HasMaxLength(100);
            b.Property(x => x.Phone).HasMaxLength(20);
            b.Property(x => x.Email).HasMaxLength(100);
            b.Property(x => x.Website).HasMaxLength(200);
            b.Property(x => x.TaxId).HasMaxLength(50);
            b.Property(x => x.RegistrationNumber).HasMaxLength(50);
            b.Property(x => x.LogoUrl).HasMaxLength(200);
            // Payroll Settings
            b.Property(x => x.PayFrequency).IsRequired();
            b.Property(x => x.StandardWorkHours).IsRequired();
            b.Property(x => x.OvertimeRate).IsRequired().HasColumnType("decimal(3,1)");
            b.Property(x => x.AutoProcessPayroll).IsRequired();
            b.HasIndex(x => x.Code).IsUnique();
            b.HasMany(x => x.Employees).WithOne(x => x.Company).HasForeignKey(x => x.CompanyId);
        });

        // Configure Employee entity
        builder.Entity<Employee>(b =>
        {
            b.ToTable(PayrollProConsts.DbTablePrefix + "Employees", PayrollProConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            b.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            b.Property(x => x.Email).IsRequired().HasMaxLength(255);
            b.Property(x => x.Phone).HasMaxLength(20);
            b.Property(x => x.EmployeeId).IsRequired().HasMaxLength(10);
            b.Property(x => x.Department).IsRequired().HasMaxLength(100);
            b.Property(x => x.Position).IsRequired().HasMaxLength(100);
            b.Property(x => x.Salary).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.Notes).HasMaxLength(500);
            b.HasIndex(x => x.EmployeeId).IsUnique();
            b.HasIndex(x => x.Email).IsUnique();
            b.HasIndex(x => x.CompanyId);
        });

        // Configure PayrollRecord entity
        builder.Entity<PayrollRecord>(b =>
        {
            b.ToTable(PayrollProConsts.DbTablePrefix + "PayrollRecords", PayrollProConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.GrossPay).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.NetPay).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.TotalDeductions).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.FederalTax).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.StateTax).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.SocialSecurityTax).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.MedicareTax).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.HealthInsurance).HasColumnType("decimal(18,2)");
            b.Property(x => x.RetirementContribution).HasColumnType("decimal(18,2)");
            b.Property(x => x.OtherDeductions).HasColumnType("decimal(18,2)");
            b.Property(x => x.RegularHours).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.OvertimeHours).HasColumnType("decimal(18,2)");
            b.Property(x => x.HourlyRate).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.OvertimeRate).HasColumnType("decimal(18,2)");
            b.Property(x => x.Notes).HasMaxLength(500);
            b.HasIndex(x => new { x.EmployeeId, x.PayPeriodStart, x.PayPeriodEnd });
        });

        // Configure Timesheet entity
        builder.Entity<Timesheet>(b =>
        {
            b.ToTable(PayrollProConsts.DbTablePrefix + "Timesheets", PayrollProConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.HoursWorked).IsRequired().HasColumnType("decimal(18,2)");
            b.Property(x => x.OvertimeHours).HasColumnType("decimal(18,2)");
            b.Property(x => x.BreakHours).HasColumnType("decimal(18,2)");
            b.Property(x => x.Notes).HasMaxLength(500);
            b.Property(x => x.ProjectCode).HasMaxLength(100);
            b.HasIndex(x => new { x.EmployeeId, x.WorkDate });
        });
    }
}
