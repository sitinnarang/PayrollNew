using System;
using System.ComponentModel.DataAnnotations;

namespace PayrollPro.Timesheets;

public class CreateUpdateTimesheetDto
{
    [Required]
    public Guid EmployeeId { get; set; }
    
    [Required]
    public DateTime WeekStarting { get; set; }
    
    [Required]
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
    
    [StringLength(500)]
    public string Notes { get; set; }
}