using System.Threading.Tasks;
using PayrollPro.Localization;
using PayrollPro.MultiTenancy;
using PayrollPro.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace PayrollPro.Web.Menus;

public class PayrollProMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<PayrollProResource>();

        // Check if the current user is a company user
        var httpContextAccessor = context.ServiceProvider.GetService(typeof(Microsoft.AspNetCore.Http.IHttpContextAccessor)) as Microsoft.AspNetCore.Http.IHttpContextAccessor;
        var user = httpContextAccessor?.HttpContext?.User;
        var isCompanyUser = user?.FindFirst("UserType")?.Value == "CompanyUser";

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                PayrollProMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        // Add PayrollPro Management section
        var payrollMenu = new ApplicationMenuItem(
            "PayrollPro",
            isCompanyUser ? "Company Management" : "PayrollPro Management",
            icon: "fas fa-briefcase",
            order: 1
        );

        // For company users, show different menu items
        if (isCompanyUser)
        {
            // Company users see company-specific menu
            var companyId = user?.FindFirst("CompanyId")?.Value;
            if (!string.IsNullOrEmpty(companyId))
            {
                payrollMenu.AddItem(
                    new ApplicationMenuItem(
                        "PayrollPro.MyCompany",
                        "My Company",
                        url: $"/Companies/{companyId}",
                        icon: "fas fa-building",
                        order: 1
                    )
                );
            }
        }
        else
        {
            // Admin users see all companies
            payrollMenu.AddItem(
                new ApplicationMenuItem(
                    "PayrollPro.Companies",
                    "Companies",
                    url: "/Companies",
                    icon: "fas fa-building",
                    order: 1
                )
            );
        }

        // Common menu items for all users
        payrollMenu.AddItem(
            new ApplicationMenuItem(
                "PayrollPro.Employees",
                "Employees", 
                url: "/Employees",
                icon: "fas fa-users",
                order: 2,
                requiredPermissionName: PayrollProPermissions.Employees.Default
            )
        ).AddItem(
            new ApplicationMenuItem(
                "PayrollPro.Payroll",
                "Payroll",
                url: "/Payroll",
                icon: "fas fa-money-bill-wave",
                order: 3
            )
        ).AddItem(
            new ApplicationMenuItem(
                "PayrollPro.Timesheets",
                "Timesheets",
                url: "/Timesheets", 
                icon: "fas fa-clock",
                order: 4
            )
        ).AddItem(
            new ApplicationMenuItem(
                "PayrollPro.Reports",
                "Reports",
                url: "/Reports",
                icon: "fas fa-chart-bar",
                order: 5
            )
        );

        context.Menu.AddItem(payrollMenu);

        // Add Admin section (only visible to admins, not company users)
        if (!isCompanyUser)
        {
            var adminMenu = new ApplicationMenuItem(
                "PayrollPro.Admin",
                "Administration",
                icon: "fas fa-crown",
                order: 2,
                requiredPermissionName: PayrollProPermissions.Admin.Default
            );

            adminMenu.AddItem(
                new ApplicationMenuItem(
                    "PayrollPro.Admin.Companies",
                    "All Companies",
                    url: "/Admin/Companies",
                    icon: "fas fa-building",
                    order: 1,
                    requiredPermissionName: PayrollProPermissions.Admin.ViewAllCompanies
                )
            );

            adminMenu.AddItem(
                new ApplicationMenuItem(
                    "PayrollPro.Admin.SystemSettings",
                    "System Settings",
                    url: "/Admin/Settings",
                    icon: "fas fa-cogs",
                    order: 2,
                    requiredPermissionName: PayrollProPermissions.Admin.Default
                )
            );

            adminMenu.AddItem(
                new ApplicationMenuItem(
                    "PayrollPro.Admin.UserManagement",
                    "User Management",
                    url: "/Admin/Users",
                    icon: "fas fa-users-cog",
                    order: 3,
                    requiredPermissionName: PayrollProPermissions.Admin.Default
                )
            );

            context.Menu.AddItem(adminMenu);
        }

        // Configure standard administration menu
        if (!isCompanyUser)
        {
            if (MultiTenancyConsts.IsEnabled)
            {
                administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
            }
            else
            {
                administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
            }

            administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
            administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
        }
        else
        {
            // Hide administration menu items for company users
            administration.Items.Clear();
        }

        return Task.CompletedTask;
    }
}
