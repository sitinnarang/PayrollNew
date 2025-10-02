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
        context.Menu.AddItem(
            new ApplicationMenuItem(
                "PayrollPro",
                "PayrollPro Management",
                icon: "fas fa-briefcase",
                order: 1
            ).AddItem(
                new ApplicationMenuItem(
                    "PayrollPro.Companies",
                    "Companies",
                    url: "/Companies",
                    icon: "fas fa-building",
                    order: 1
                )
            ).AddItem(
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
            )
        );

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

        return Task.CompletedTask;
    }
}
