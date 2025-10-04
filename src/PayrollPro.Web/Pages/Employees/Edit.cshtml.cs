using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Employees;

namespace PayrollPro.Web.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly IEmployeeAppService _employeeAppService;

        public EditModel(IEmployeeAppService employeeAppService)
        {
            _employeeAppService = employeeAppService;
        }

        [BindProperty]
        public CreateUpdateEmployeeDto Employee { get; set; } = new();

        [BindProperty]
        public Guid EmployeeId { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                EmployeeId = id;
                var employee = await _employeeAppService.GetAsync(id);
                
                Employee = new CreateUpdateEmployeeDto
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    Email = employee.Email,
                    Phone = employee.Phone,
                    EmployeeId = employee.EmployeeId,
                    Department = employee.Department,
                    Position = employee.Position,
                    Salary = employee.Salary,
                    HireDate = employee.HireDate,
                    Status = employee.Status,
                    Notes = employee.Notes,
                    CompanyId = employee.CompanyId
                };

                return Page();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the validation errors and try again.";
                return Page();
            }

            try
            {
                await _employeeAppService.UpdateAsync(EmployeeId, Employee);
                
                TempData["SuccessMessage"] = $"Employee {Employee.FirstName} {Employee.LastName} has been updated successfully!";
                return RedirectToPage("./Edit", new { id = EmployeeId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to update employee: {ex.Message}";
                ModelState.AddModelError("", $"An error occurred while updating the employee: {ex.Message}");
                return Page();
            }
        }
    }
}