namespace PayrollPro.Permissions;

public static class PayrollProPermissions
{
    public const string GroupName = "PayrollPro";

    public static class Employees
    {
        public const string Default = GroupName + ".Employees";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Payrolls
    {
        public const string Default = GroupName + ".Payrolls";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Process = Default + ".Process";
    }

    public static class Timesheets
    {
        public const string Default = GroupName + ".Timesheets";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Approve = Default + ".Approve";
    }

    public static class Reports
    {
        public const string Default = GroupName + ".Reports";
        public const string PayrollReports = Default + ".PayrollReports";
        public const string EmployeeReports = Default + ".EmployeeReports";
        public const string TimesheetReports = Default + ".TimesheetReports";
    }
}
