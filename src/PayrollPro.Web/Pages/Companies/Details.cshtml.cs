using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Companies;
using PayrollPro.Payrolls;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace PayrollPro.Web.Pages.Companies
{
    public class DetailsModel : AbpPageModel
    {
        private readonly ICompanyAppService _companyAppService;

        public DetailsModel(ICompanyAppService companyAppService)
        {
            _companyAppService = companyAppService;
        }

        [BindProperty]
        public CompanyDto Company { get; set; } = null!;

        [BindProperty]
        public PayrollSettingsDto PayrollSettings { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                Company = await _companyAppService.GetAsync(id);
                
                if (Company == null)
                {
                    return NotFound();
                }

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