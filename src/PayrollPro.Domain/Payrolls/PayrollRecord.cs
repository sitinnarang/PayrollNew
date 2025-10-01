using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace PayrollPro.Payrolls
{
    public class PayrollRecord : FullAuditedAggregateRoot<Guid>
    {
        [Required]
        public Guid EmployeeId { get; set; }

        [Required]
        public DateTime PayPeriodStart { get; set; }

        [Required]
        public DateTime PayPeriodEnd { get; set; }

        [Required]
        public DateTime PayDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal GrossPay { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal NetPay { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalDeductions { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal FederalTax { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal StateTax { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal SocialSecurityTax { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MedicareTax { get; set; }

        [Range(0, double.MaxValue)]
        public decimal HealthInsurance { get; set; }

        [Range(0, double.MaxValue)]
        public decimal RetirementContribution { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OtherDeductions { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal RegularHours { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OvertimeHours { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal HourlyRate { get; set; }

        [Range(0, double.MaxValue)]
        public decimal OvertimeRate { get; set; }

        [Required]
        public PayrollStatus Status { get; set; } = PayrollStatus.Draft;

        [StringLength(500)]
        public string? Notes { get; set; }

        protected PayrollRecord()
        {
        }

        public PayrollRecord(
            Guid id,
            Guid employeeId,
            DateTime payPeriodStart,
            DateTime payPeriodEnd,
            DateTime payDate,
            decimal regularHours,
            decimal hourlyRate,
            decimal overtimeHours = 0,
            decimal overtimeRate = 0) : base(id)
        {
            EmployeeId = employeeId;
            PayPeriodStart = payPeriodStart;
            PayPeriodEnd = payPeriodEnd;
            PayDate = payDate;
            RegularHours = regularHours;
            HourlyRate = hourlyRate;
            OvertimeHours = overtimeHours;
            OvertimeRate = overtimeRate;
            Status = PayrollStatus.Draft;

            CalculatePayroll();
        }

        public void CalculatePayroll()
        {
            // Calculate gross pay
            GrossPay = (RegularHours * HourlyRate) + (OvertimeHours * OvertimeRate);

            // Calculate taxes (simplified calculation)
            FederalTax = GrossPay * 0.22m; // 22% federal tax
            StateTax = GrossPay * 0.05m; // 5% state tax
            SocialSecurityTax = GrossPay * 0.062m; // 6.2% Social Security
            MedicareTax = GrossPay * 0.0145m; // 1.45% Medicare

            // Calculate total deductions
            TotalDeductions = FederalTax + StateTax + SocialSecurityTax + MedicareTax + 
                            HealthInsurance + RetirementContribution + OtherDeductions;

            // Calculate net pay
            NetPay = GrossPay - TotalDeductions;
        }

        public void UpdateBenefits(decimal healthInsurance, decimal retirementContribution, decimal otherDeductions)
        {
            HealthInsurance = healthInsurance;
            RetirementContribution = retirementContribution;
            OtherDeductions = otherDeductions;
            CalculatePayroll();
        }

        public void UpdateStatus(PayrollStatus status)
        {
            Status = status;
        }
    }
}