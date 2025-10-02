using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Companies;

namespace PayrollPro.Web.Pages.Companies
{
    public class CreateModalModel : PageModel
    {
        private readonly ICompanyAppService _companyAppService;

        public CreateModalModel(ICompanyAppService companyAppService)
        {
            _companyAppService = companyAppService;
        }

        [BindProperty]
        public CreateUpdateCompanyDto Company { get; set; } = new();

        public void OnGet()
        {
            // Initialize with default values
            Company = new CreateUpdateCompanyDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new JsonResult(new { success = false, errors = ModelState });
                }

                await _companyAppService.CreateAsync(Company);
                return new JsonResult(new { success = true });
            }
            catch (System.Exception ex)
            {
                return new JsonResult(new { success = false, error = ex.Message });
            }
        }
    }
}