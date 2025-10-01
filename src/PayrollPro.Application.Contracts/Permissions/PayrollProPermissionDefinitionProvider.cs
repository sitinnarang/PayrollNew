using PayrollPro.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace PayrollPro.Permissions;

public class PayrollProPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(PayrollProPermissions.GroupName);

        // Employee permissions
        var employeesPermission = myGroup.AddPermission(PayrollProPermissions.Employees.Default, L("Permission:Employees"));
        employeesPermission.AddChild(PayrollProPermissions.Employees.Create, L("Permission:Employees.Create"));
        employeesPermission.AddChild(PayrollProPermissions.Employees.Edit, L("Permission:Employees.Edit"));
        employeesPermission.AddChild(PayrollProPermissions.Employees.Delete, L("Permission:Employees.Delete"));

        // Payroll permissions
        var payrollsPermission = myGroup.AddPermission(PayrollProPermissions.Payrolls.Default, L("Permission:Payrolls"));
        payrollsPermission.AddChild(PayrollProPermissions.Payrolls.Create, L("Permission:Payrolls.Create"));
        payrollsPermission.AddChild(PayrollProPermissions.Payrolls.Edit, L("Permission:Payrolls.Edit"));
        payrollsPermission.AddChild(PayrollProPermissions.Payrolls.Delete, L("Permission:Payrolls.Delete"));
        payrollsPermission.AddChild(PayrollProPermissions.Payrolls.Process, L("Permission:Payrolls.Process"));

        // Timesheet permissions
        var timesheetsPermission = myGroup.AddPermission(PayrollProPermissions.Timesheets.Default, L("Permission:Timesheets"));
        timesheetsPermission.AddChild(PayrollProPermissions.Timesheets.Create, L("Permission:Timesheets.Create"));
        timesheetsPermission.AddChild(PayrollProPermissions.Timesheets.Edit, L("Permission:Timesheets.Edit"));
        timesheetsPermission.AddChild(PayrollProPermissions.Timesheets.Delete, L("Permission:Timesheets.Delete"));
        timesheetsPermission.AddChild(PayrollProPermissions.Timesheets.Approve, L("Permission:Timesheets.Approve"));

        // Reports permissions
        var reportsPermission = myGroup.AddPermission(PayrollProPermissions.Reports.Default, L("Permission:Reports"));
        reportsPermission.AddChild(PayrollProPermissions.Reports.PayrollReports, L("Permission:Reports.PayrollReports"));
        reportsPermission.AddChild(PayrollProPermissions.Reports.EmployeeReports, L("Permission:Reports.EmployeeReports"));
        reportsPermission.AddChild(PayrollProPermissions.Reports.TimesheetReports, L("Permission:Reports.TimesheetReports"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<PayrollProResource>(name);
    }
}
