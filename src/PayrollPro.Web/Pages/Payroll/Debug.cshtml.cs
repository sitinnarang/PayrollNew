using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Employees;
using PayrollPro.Companies;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Application.Dtos;

namespace PayrollPro.Web.Pages.Payroll
{
    public class DebugModel : AbpPageModel
    {
        private readonly IEmployeeAppService _employeeAppService;
        private readonly ICompanyAppService _companyAppService;

        public DebugModel(
            IEmployeeAppService employeeAppService,
            ICompanyAppService companyAppService)
        {
            _employeeAppService = employeeAppService;
            _companyAppService = companyAppService;
        }

        public IReadOnlyList<CompanyDto> Companies { get; set; } = new List<CompanyDto>();
        public IReadOnlyList<EmployeeDto> Employees { get; set; } = new List<EmployeeDto>();

        public async Task OnGetAsync()
        {
            try
            {
                var companiesResult = await _companyAppService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 100 });
                Companies = companiesResult.Items;

                var employeesResult = await _employeeAppService.GetListAsync(new PagedAndSortedResultRequestDto { MaxResultCount = 100 });
                Employees = employeesResult.Items;
            }
            catch (System.Exception)
            {
                // If there's an error, just leave the lists empty
                Companies = new List<CompanyDto>();
                Employees = new List<EmployeeDto>();
            }
        }
    }
}