using System;
using System.ComponentModel.DataAnnotations;

namespace PayrollPro.Companies
{
    public class PayrollSettingsDto
    {
        [Required]
        public PayFrequency PayFrequency { get; set; } = PayFrequency.Monthly;
        
        public DateTime? PayPeriodEnd { get; set; }
        
        [Required]
        [Range(1, 60, ErrorMessage = "Standard work hours must be between 1 and 60")]
        public int StandardWorkHours { get; set; } = 40;
        
        [Required]
        [Range(1.0, 5.0, ErrorMessage = "Overtime rate must be between 1.0 and 5.0")]
        public decimal OvertimeRate { get; set; } = 1.5m;
        
        public bool AutoProcessPayroll { get; set; } = false;
    }
}