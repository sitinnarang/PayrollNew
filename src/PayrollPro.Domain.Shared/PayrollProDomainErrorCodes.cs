namespace PayrollPro;

public static class PayrollProDomainErrorCodes
{
    /* You can add your business exception error codes here, as constants */
    
    // Employee error codes
    public const string EmployeeIdAlreadyExists = "PayrollPro:010001";
    public const string EmployeeEmailAlreadyExists = "PayrollPro:010002";
    public const string EmployeeNotFound = "PayrollPro:010003";

    // Payroll error codes
    public const string PayrollRecordAlreadyExists = "PayrollPro:020001";
    public const string PayrollPeriodOverlap = "PayrollPro:020002";
    public const string CannotProcessPayroll = "PayrollPro:020003";

    // Timesheet error codes
    public const string TimesheetAlreadyExists = "PayrollPro:030001";
    public const string TimesheetAlreadyApproved = "PayrollPro:030002";
    public const string InvalidTimeRange = "PayrollPro:030003";
}
