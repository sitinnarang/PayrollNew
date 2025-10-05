using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Companies;
using PayrollPro.Employees;
using Volo.Abp.Application.Dtos;

namespace PayrollPro.Web.Pages.Employees
{
    public class LeaveModel : PageModel
    {
        private readonly IEmployeeAppService _employeeAppService;
        private readonly ICompanyAppService _companyAppService;

        public LeaveModel(IEmployeeAppService employeeAppService, ICompanyAppService companyAppService)
        {
            _employeeAppService = employeeAppService;
            _companyAppService = companyAppService;
        }

        public ListResultDto<EmployeeDto> Employees { get; set; } = new();
        public PagedResultDto<CompanyDto> Companies { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? CompanyId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Department { get; set; }

        public async Task OnGetAsync()
        {
            try
            {
                Console.WriteLine("DEBUG: OnLeave page - Loading companies and on-leave employees");
                
                // Load all companies for the filter dropdown
                Companies = await _companyAppService.GetListAsync(new PagedAndSortedResultRequestDto
                {
                    MaxResultCount = 1000
                });
                Console.WriteLine($"DEBUG: OnLeave page - Companies loaded: {Companies.Items.Count}");

                // Get all employees on leave
                Employees = await _employeeAppService.GetEmployeesByStatusAsync(EmployeeStatus.OnLeave);
                Console.WriteLine($"DEBUG: OnLeave page - Employees on leave loaded: {Employees.Items.Count}");

                // Apply company filter if specified
                if (!string.IsNullOrEmpty(CompanyId) && Guid.TryParse(CompanyId, out var companyGuid))
                {
                    Console.WriteLine($"DEBUG: OnLeave page - Filtering by company: {CompanyId}");
                    var filteredByCompany = Employees.Items.Where(e => e.CompanyId == companyGuid).ToList();
                    Employees = new ListResultDto<EmployeeDto>(filteredByCompany);
                    Console.WriteLine($"DEBUG: OnLeave page - After company filter: {Employees.Items.Count}");
                }

                // Apply search filter if specified
                if (!string.IsNullOrEmpty(Search))
                {
                    Console.WriteLine($"DEBUG: OnLeave page - Applying search filter: {Search}");
                    var filteredBySearch = Employees.Items.Where(e => 
                        e.FullName.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        e.EmployeeId.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        e.Department.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        e.Position.Contains(Search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                    Employees = new ListResultDto<EmployeeDto>(filteredBySearch);
                    Console.WriteLine($"DEBUG: OnLeave page - After search filter: {Employees.Items.Count}");
                }

                // Apply department filter if specified
                if (!string.IsNullOrEmpty(Department))
                {
                    Console.WriteLine($"DEBUG: OnLeave page - Applying department filter: {Department}");
                    var filteredByDept = Employees.Items.Where(e => 
                        e.Department.Equals(Department, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                    Employees = new ListResultDto<EmployeeDto>(filteredByDept);
                    Console.WriteLine($"DEBUG: OnLeave page - After department filter: {Employees.Items.Count}");
                }

                // Log employee details for debugging
                foreach (var employee in Employees.Items)
                {
                    Console.WriteLine($"DEBUG: OnLeave Employee - {employee.FullName} ({employee.EmployeeId}) - Status: {employee.Status} - Company: {employee.CompanyId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: OnLeave page error: {ex.Message}");
                // Initialize empty collections to prevent page errors
                Employees = new ListResultDto<EmployeeDto>();
                Companies = new PagedResultDto<CompanyDto>();
            }
        }

        public async Task<IActionResult> OnPostMarkAsReturnedAsync(Guid employeeId)
        {
            try
            {
                var employee = await _employeeAppService.GetAsync(employeeId);
                if (employee != null)
                {
                    var updateDto = new CreateUpdateEmployeeDto
                    {
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        Phone = employee.Phone,
                        EmployeeId = employee.EmployeeId,
                        Position = employee.Position,
                        Department = employee.Department,
                        HireDate = employee.HireDate,
                        Salary = employee.Salary,
                        Status = EmployeeStatus.Active, // Change status back to Active
                        Notes = employee.Notes,
                        CompanyId = employee.CompanyId
                    };

                    await _employeeAppService.UpdateAsync(employeeId, updateDto);
                    Console.WriteLine($"DEBUG: Employee {employee.FullName} marked as returned from leave");
                }

                return new JsonResult(new { success = true, message = "Employee marked as returned successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DEBUG: Error marking employee as returned: {ex.Message}");
                return new JsonResult(new { success = false, message = "Error updating employee status" });
            }
        }
    }
}