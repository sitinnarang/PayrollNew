using System;
using Volo.Abp.Application.Dtos;

namespace PayrollPro.Timesheets;

public class TimesheetDto : EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime WeekStarting { get; set; }
    public DateTime WeekEnding { get; set; }
    public TimeSpan? MondayStart { get; set; }
    public TimeSpan? MondayEnd { get; set; }
    public TimeSpan? TuesdayStart { get; set; }
    public TimeSpan? TuesdayEnd { get; set; }
    public TimeSpan? WednesdayStart { get; set; }
    public TimeSpan? WednesdayEnd { get; set; }
    public TimeSpan? ThursdayStart { get; set; }
    public TimeSpan? ThursdayEnd { get; set; }
    public TimeSpan? FridayStart { get; set; }
    public TimeSpan? FridayEnd { get; set; }
    public TimeSpan? SaturdayStart { get; set; }
    public TimeSpan? SaturdayEnd { get; set; }
    public TimeSpan? SundayStart { get; set; }
    public TimeSpan? SundayEnd { get; set; }
    public decimal TotalRegularHours { get; set; }
    public decimal TotalOvertimeHours { get; set; }
    public TimesheetStatus Status { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public Guid? ApprovedBy { get; set; }
    public string ApproverName { get; set; }
    public string Notes { get; set; }
}