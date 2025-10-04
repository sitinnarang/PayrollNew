using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using PayrollPro.Employees;
using System;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace PayrollPro.Web.Pages.Payroll
{
    public class EmployeeModel : AbpPageModel
    {
        [BindProperty]
        public EmployeeDto? Employee { get; set; }

        private readonly IEmployeeAppService _employeeAppService;

        public EmployeeModel(IEmployeeAppService employeeAppService)
        {
            _employeeAppService = employeeAppService;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                Employee = await _employeeAppService.GetAsync(id);
                return Page();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error retrieving employee for payroll view: {EmployeeId}", id);
                return RedirectToPage("/Employees/Index");
            }
        }
    }
}