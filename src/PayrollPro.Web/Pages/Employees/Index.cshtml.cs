using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Application.Dtos;
using PayrollPro.Companies;
using PayrollPro.Employees;

namespace PayrollPro.Web.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly IEmployeeAppService _employeeAppService;
        private readonly ICompanyAppService _companyAppService;

        public IndexModel(
            IEmployeeAppService employeeAppService,
            ICompanyAppService companyAppService)
        {
            _employeeAppService = employeeAppService;
            _companyAppService = companyAppService;
        }

        public PagedResultDto<EmployeeDto> Employees { get; set; } = new();
        public PagedResultDto<CompanyDto> Companies { get; set; } = new();

        public async Task OnGetAsync(Guid? companyId = null)
        {
            // Get all companies for dropdown
            Companies = await _companyAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = 1000
            });

            Console.WriteLine($"DEBUG: Companies loaded: {Companies.TotalCount}");
            if (Companies.Items.Any())
            {
                Console.WriteLine($"DEBUG: First company: {Companies.Items.First().Name} (ID: {Companies.Items.First().Id})");
            }

            // Get employees filtered by company if specified
            if (companyId.HasValue)
            {
                Console.WriteLine($"DEBUG: Loading employees for company ID: {companyId.Value}");
                Employees = await _employeeAppService.GetEmployeesByCompanyAsync(companyId.Value, new PagedAndSortedResultRequestDto
                {
                    MaxResultCount = 1000
                });
                Console.WriteLine($"DEBUG: Employees loaded: {Employees.TotalCount}");
            }
            else
            {
                Console.WriteLine("DEBUG: Loading all employees (no company filter)");
                // Get all employees if no company filter
                Employees = await _employeeAppService.GetListAsync(new PagedAndSortedResultRequestDto
                {
                    MaxResultCount = 1000
                });
                Console.WriteLine($"DEBUG: All employees loaded: {Employees.TotalCount}");
            }
        }
    }
}