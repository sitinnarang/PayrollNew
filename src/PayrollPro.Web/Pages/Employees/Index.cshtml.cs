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

            // Get employees
            Employees = await _employeeAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = 1000
            });

            // Filter by company if specified
            if (companyId.HasValue)
            {
                var filteredEmployees = Employees.Items.Where(e => e.CompanyId == companyId.Value).ToList();
                Employees = new PagedResultDto<EmployeeDto>(
                    filteredEmployees.Count,
                    filteredEmployees
                );
            }
        }
    }
}