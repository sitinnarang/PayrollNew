using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Companies;
using System.Threading.Tasks;
using System;
using System.Linq;
using Volo.Abp.Identity;
using System.ComponentModel.DataAnnotations;

namespace PayrollPro.Web.Pages.Companies
{
    public class CreateCompanyModel : PageModel
    {
        private readonly ICompanyAppService _companyAppService;
        private readonly IIdentityUserAppService _identityUserAppService;

        public CreateCompanyModel(
            ICompanyAppService companyAppService,
            IIdentityUserAppService identityUserAppService)
        {
            _companyAppService = companyAppService;
            _identityUserAppService = identityUserAppService;
        }

        [BindProperty]
        public CreateUpdateCompanyDto Company { get; set; } = new CreateUpdateCompanyDto();

        [BindProperty]
        public UserRegistrationDto UserRegistration { get; set; } = new UserRegistrationDto();

        public void OnGet()
        {
            // Initialize default values
            Company.EstablishedDate = DateTime.Now.Date;
            Company.IsActive = true;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    
                    return new JsonResult(new { success = false, error = errors });
                }

                // First create the user account
                var userCreateDto = new IdentityUserCreateDto
                {
                    UserName = UserRegistration.Username,
                    Email = UserRegistration.Email,
                    Password = UserRegistration.Password,
                    IsActive = true,
                    LockoutEnabled = false
                };

                var createdUser = await _identityUserAppService.CreateAsync(userCreateDto);

                // Then create the company in the database
                var createdCompany = await _companyAppService.CreateAsync(Company);

                // Return success response
                return new JsonResult(new { 
                    success = true, 
                    companyId = createdCompany.Id,
                    userId = createdUser.Id,
                    message = "Account and company created successfully!" 
                });
            }
            catch (Exception ex)
            {
                // Log the error (you might want to use ILogger here)
                return new JsonResult(new { 
                    success = false, 
                    error = $"Failed to create account and company: {ex.Message}" 
                });
            }
        }
    }

    public class UserRegistrationDto
    {
        [Required]
        [StringLength(256)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(128)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}