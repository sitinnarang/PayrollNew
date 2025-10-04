using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Employees;

namespace PayrollPro.Web.Pages.Employees
{
    public class DetailsModel : PageModel
    {
        private readonly IEmployeeAppService _employeeAppService;

        public DetailsModel(IEmployeeAppService employeeAppService)
        {
            _employeeAppService = employeeAppService;
        }

        public EmployeeDto Employee { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                Employee = await _employeeAppService.GetAsync(id);
                return Page();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}