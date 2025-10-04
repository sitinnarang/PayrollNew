using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Employees;

namespace PayrollPro.Web.Pages.Employees
{
    public class DeleteModel : PageModel
    {
        private readonly IEmployeeAppService _employeeAppService;

        public DeleteModel(IEmployeeAppService employeeAppService)
        {
            _employeeAppService = employeeAppService;
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            try
            {
                // Get employee details before deletion to return to correct company
                var employee = await _employeeAppService.GetAsync(id);
                var companyId = employee.CompanyId;

                await _employeeAppService.DeleteAsync(id);
                
                return Redirect($"/Employees?companyId={companyId}");
            }
            catch (Exception)
            {
                // If deletion fails, redirect back to employee list
                return Redirect("/Employees");
            }
        }
    }
}