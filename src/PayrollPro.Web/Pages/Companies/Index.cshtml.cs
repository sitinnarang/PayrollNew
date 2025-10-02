using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.Application.Dtos;
using PayrollPro.Companies;

namespace PayrollPro.Web.Pages.Companies
{
    public class IndexModel : PageModel
    {
        private readonly ICompanyAppService _companyAppService;

        public IndexModel(ICompanyAppService companyAppService)
        {
            _companyAppService = companyAppService;
        }

        public PagedResultDto<CompanyDto> Companies { get; set; } = new();

        public async Task OnGetAsync()
        {
            Companies = await _companyAppService.GetListAsync(new PagedAndSortedResultRequestDto
            {
                MaxResultCount = 1000 // Get all companies for now
            });
        }
    }
}