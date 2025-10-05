using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Companies;
using PayrollPro.Employees;
using PayrollPro.Payrolls;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace PayrollPro.Web.Pages.Companies
{
    public class DetailsModel : AbpPageModel
    {
        private readonly ICompanyAppService _companyAppService;
        private readonly IEmployeeAppService _employeeAppService;

        public DetailsModel(ICompanyAppService companyAppService, IEmployeeAppService employeeAppService)
        {
            _companyAppService = companyAppService;
            _employeeAppService = employeeAppService;
        }

        [BindProperty]
        public CompanyDto Company { get; set; } = null!;

        [BindProperty]
        public PayrollSettingsDto PayrollSettings { get; set; } = null!;

        public int ActiveEmployeesCount { get; set; }
        public int OnLeaveEmployeesCount { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                Company = await _companyAppService.GetAsync(id);
                
                if (Company == null)
                {
                    return NotFound();
                }

                // Get employee counts by status for this company
                var companyEmployees = await _employeeAppService.GetEmployeesByCompanyAsync(id, new PagedAndSortedResultRequestDto { MaxResultCount = 1000 });
                ActiveEmployeesCount = companyEmployees.Items.Count(e => e.Status == EmployeeStatus.Active);
                OnLeaveEmployeesCount = companyEmployees.Items.Count(e => e.Status == EmployeeStatus.OnLeave);

                // Load payroll settings
                try
                {
                    PayrollSettings = await _companyAppService.GetPayrollSettingsAsync(id);
                }
                catch
                {
                    // If no settings exist, initialize with defaults
                    PayrollSettings = new PayrollSettingsDto
                    {
                        PayFrequency = PayFrequency.Monthly,
                        StandardWorkHours = 40,
                        OvertimeRate = 1.5m,
                        AutoProcessPayroll = false,
                        PayPeriodEnd = DateTime.Today.AddDays(30)
                    };
                }

                return Page();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            try
            {
                await _companyAppService.DeleteAsync(Company.Id);
                return RedirectToPage("./Index");
            }
            catch (Exception)
            {
                // Log the exception if you have logging configured
                // Add error message to be displayed to user
                return Page();
            }
        }

        public async Task<IActionResult> OnPostSavePayrollSettingsAsync()
        {
            try
            {
                await _companyAppService.UpdatePayrollSettingsAsync(Company.Id, PayrollSettings);
                return new JsonResult(new { success = true, message = "Payroll settings saved successfully!" });
            }
            catch (Exception)
            {
                return new JsonResult(new { success = false, message = "Error saving payroll settings." });
            }
        }
    }
}