using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Companies;
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
        public CompanyDto Company { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                Company = await _companyAppService.GetAsync(id);
                
                if (Company == null)
                {
                    return NotFound();
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
            catch (Exception ex)
            {
                // Log the exception if you have logging configured
                // Add error message to be displayed to user
                return Page();
            }
        }
    }
}