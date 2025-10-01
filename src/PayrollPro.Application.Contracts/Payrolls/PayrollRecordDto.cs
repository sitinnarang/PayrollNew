using System;
using Volo.Abp.Application.Dtos;

namespace PayrollPro.Payrolls;

public class PayrollRecordDto : EntityDto<Guid>
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public decimal RegularHours { get; set; }
    public decimal OvertimeHours { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal RegularPay { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal GrossPay { get; set; }
    public decimal FederalTax { get; set; }
    public decimal StateTax { get; set; }
    public decimal FicaTax { get; set; }
    public decimal MedicareTax { get; set; }
    public decimal HealthInsurance { get; set; }
    public decimal RetirementContribution { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetPay { get; set; }
    public PayrollStatus Status { get; set; }
    public DateTime? ProcessedDate { get; set; }
    public string Notes { get; set; }
}