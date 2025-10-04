using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Companies;
using System.Threading.Tasks;
using System;
using System.Linq;
using Volo.Abp.Identity;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Account;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Identity;

namespace PayrollPro.Web.Pages.Companies
{
    [AllowAnonymous]
    public class CreateCompanyModel : PageModel
    {
        private readonly ICompanyAppService _companyAppService;
        private readonly IIdentityUserAppService _identityUserAppService;
        private readonly SignInManager<Volo.Abp.Identity.IdentityUser> _signInManager;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;

        public CreateCompanyModel(
            ICompanyAppService companyAppService,
            IIdentityUserAppService identityUserAppService,
            SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager)
        {
            _companyAppService = companyAppService;
            _identityUserAppService = identityUserAppService;
            _signInManager = signInManager;
            _userManager = userManager;
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

                // Automatically sign in the user after successful registration
                var user = await _userManager.FindByNameAsync(createdUser.UserName);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                }

                // Return success response with redirect
                return new JsonResult(new { 
                    success = true, 
                    companyId = createdCompany.Id,
                    userId = createdUser.Id,
                    message = "Company and user account created successfully! You are now logged in.",
                    redirectUrl = "/" // Redirect to home page after login
                });
            }
            catch (Exception ex)
            {
                // Log the error (you might want to use ILogger here)
                return new JsonResult(new { 
                    success = false, 
                    error = $"Failed to create company or user: {ex.Message}" 
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