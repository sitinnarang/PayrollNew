using System;
using System.ComponentModel.DataAnnotations;

namespace PayrollPro.Payrolls;

public class CreateUpdatePayrollRecordDto
{
    [Required]
    public Guid EmployeeId { get; set; }
    
    [Required]
    public DateTime PayPeriodStart { get; set; }
    
    [Required]
    public DateTime PayPeriodEnd { get; set; }
    
    [Range(0, 168, ErrorMessage = "Regular hours must be between 0 and 168")]
    public decimal RegularHours { get; set; }
    
    [Range(0, 100, ErrorMessage = "Overtime hours must be between 0 and 100")]
    public decimal OvertimeHours { get; set; }
    
    [Range(0.01, 999.99, ErrorMessage = "Hourly rate must be between 0.01 and 999.99")]
    public decimal HourlyRate { get; set; }
    
    [Range(0, 9999.99, ErrorMessage = "Health insurance must be between 0 and 9999.99")]
    public decimal HealthInsurance { get; set; }
    
    [Range(0, 9999.99, ErrorMessage = "Retirement contribution must be between 0 and 9999.99")]
    public decimal RetirementContribution { get; set; }
    
    [StringLength(500)]
    public string Notes { get; set; }
}