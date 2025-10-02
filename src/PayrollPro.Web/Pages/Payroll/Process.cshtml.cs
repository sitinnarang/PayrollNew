using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PayrollPro.Employees;
using PayrollPro.Companies;
using PayrollPro.Payrolls;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace PayrollPro.Web.Pages.Payroll
{
    public class ProcessModel : AbpPageModel
    {
        private readonly IEmployeeAppService _employeeAppService;
        private readonly ICompanyAppService _companyAppService;

        public ProcessModel(
            IEmployeeAppService employeeAppService,
            ICompanyAppService companyAppService)
        {
            _employeeAppService = employeeAppService;
            _companyAppService = companyAppService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid CompanyId { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid EmployeeId { get; set; }

        public EmployeeDto Employee { get; set; } = null!;
        public CompanyDto Company { get; set; } = null!;
        public PayrollCalculationDto PayrollCalculation { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (CompanyId == Guid.Empty || EmployeeId == Guid.Empty)
            {
                return BadRequest("Company ID and Employee ID are required.");
            }

            try
            {
                Employee = await _employeeAppService.GetAsync(EmployeeId);
                Company = await _companyAppService.GetAsync(CompanyId);
                
                // Initialize payroll calculation with default values
                PayrollCalculation = new PayrollCalculationDto
                {
                    EmployeeId = EmployeeId,
                    CompanyId = CompanyId,
                    RegularHours = 80.00m,
                    OvertimeHours = 0.00m,
                    RegularRate = Employee.Salary / 2080, // Assuming annual salary / 2080 hours
                    OvertimeRate = (Employee.Salary / 2080) * 1.5m
                };

                CalculatePayroll();
                
                return Page();
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Logger.LogError(ex, "Error loading payroll data for Company: {CompanyId}, Employee: {EmployeeId}", CompanyId, EmployeeId);
                return NotFound($"Could not find employee or company. Company ID: {CompanyId}, Employee ID: {EmployeeId}");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Employee = await _employeeAppService.GetAsync(EmployeeId);
                Company = await _companyAppService.GetAsync(CompanyId);
                return Page();
            }

            CalculatePayroll();
            
            // Here you would typically save the payroll to database
            // await _payrollAppService.CreateAsync(PayrollCalculation);
            
            return Page();
        }

        private void CalculatePayroll()
        {
            // Calculate gross pay
            PayrollCalculation.RegularPay = PayrollCalculation.RegularHours * PayrollCalculation.RegularRate;
            PayrollCalculation.OvertimePay = PayrollCalculation.OvertimeHours * PayrollCalculation.OvertimeRate;
            PayrollCalculation.GrossPay = PayrollCalculation.RegularPay + PayrollCalculation.OvertimePay;

            // Calculate taxes (simplified calculations)
            PayrollCalculation.FederalIncomeTax = PayrollCalculation.GrossPay * 0.12m; // 12% federal tax
            PayrollCalculation.SocialSecurity = PayrollCalculation.GrossPay * 0.062m; // 6.2% Social Security
            PayrollCalculation.Medicare = PayrollCalculation.GrossPay * 0.0145m; // 1.45% Medicare
            PayrollCalculation.StateIncomeTax = PayrollCalculation.GrossPay * 0.05m; // 5% state tax
            
            PayrollCalculation.TotalTaxes = PayrollCalculation.FederalIncomeTax + 
                                          PayrollCalculation.SocialSecurity + 
                                          PayrollCalculation.Medicare + 
                                          PayrollCalculation.StateIncomeTax;

            PayrollCalculation.NetPay = PayrollCalculation.GrossPay - PayrollCalculation.TotalTaxes;

            // YTD calculations (simplified - would normally come from database)
            PayrollCalculation.RegularPayYTD = PayrollCalculation.RegularPay * 12; // Assume 12 pay periods
            PayrollCalculation.OvertimePayYTD = PayrollCalculation.OvertimePay * 12;
            PayrollCalculation.GrossPayYTD = PayrollCalculation.GrossPay * 12;
            PayrollCalculation.FederalIncomeTaxYTD = PayrollCalculation.FederalIncomeTax * 12;
            PayrollCalculation.SocialSecurityYTD = PayrollCalculation.SocialSecurity * 12;
            PayrollCalculation.MedicareYTD = PayrollCalculation.Medicare * 12;
            PayrollCalculation.StateIncomeTaxYTD = PayrollCalculation.StateIncomeTax * 12;
            PayrollCalculation.TotalTaxesYTD = PayrollCalculation.TotalTaxes * 12;
            PayrollCalculation.NetPayYTD = PayrollCalculation.NetPay * 12;
        }
    }

    public class PayrollCalculationDto
    {
        public Guid EmployeeId { get; set; }
        public Guid CompanyId { get; set; }
        
        // Hours and Rates
        public decimal RegularHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal RegularRate { get; set; }
        public decimal OvertimeRate { get; set; }
        
        // Pay Calculations - Current
        public decimal RegularPay { get; set; }
        public decimal OvertimePay { get; set; }
        public decimal GrossPay { get; set; }
        
        // Tax Calculations - Current
        public decimal FederalIncomeTax { get; set; }
        public decimal SocialSecurity { get; set; }
        public decimal Medicare { get; set; }
        public decimal StateIncomeTax { get; set; }
        public decimal TotalTaxes { get; set; }
        
        public decimal NetPay { get; set; }
        
        // YTD Amounts
        public decimal RegularPayYTD { get; set; }
        public decimal OvertimePayYTD { get; set; }
        public decimal GrossPayYTD { get; set; }
        public decimal FederalIncomeTaxYTD { get; set; }
        public decimal SocialSecurityYTD { get; set; }
        public decimal MedicareYTD { get; set; }
        public decimal StateIncomeTaxYTD { get; set; }
        public decimal TotalTaxesYTD { get; set; }
        public decimal NetPayYTD { get; set; }
    }
}