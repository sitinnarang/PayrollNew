using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PayrollPro.Companies;
using System.Threading.Tasks;
using System;
using System.Linq;
using Volo.Abp.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Domain.Repositories;

namespace PayrollPro.Web.Pages.Companies
{
    [AllowAnonymous]
    public class CreateCompanyModel : PageModel
    {
        private readonly ICompanyAppService _companyAppService;
        private readonly SignInManager<Volo.Abp.Identity.IdentityUser> _signInManager;
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly IRepository<Company, Guid> _companyRepository;

        public CreateCompanyModel(
            ICompanyAppService companyAppService,
            SignInManager<Volo.Abp.Identity.IdentityUser> signInManager,
            UserManager<Volo.Abp.Identity.IdentityUser> userManager,
            IRepository<Company, Guid> companyRepository)
        {
            _companyAppService = companyAppService;
            _signInManager = signInManager;
            _userManager = userManager;
            _companyRepository = companyRepository;
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

                // First create the company, then we'll create the user
                // (Testing company creation first)

                // Then create the company in the database (using repository to avoid authorization)
                var company = new Company
                {
                    Name = Company.Name,
                    Code = Company.Code ?? await GenerateCompanyCodeAsync(Company.Name),
                    Description = Company.Description,
                    Address = Company.Address,
                    City = Company.City,
                    State = Company.State,
                    ZipCode = Company.ZipCode,
                    Country = Company.Country,
                    Phone = Company.Phone,
                    Email = Company.Email,
                    Website = Company.Website,
                    TaxId = Company.TaxId,
                    RegistrationNumber = Company.RegistrationNumber,
                    EstablishedDate = Company.EstablishedDate,
                    IsActive = Company.IsActive,
                    LogoUrl = Company.LogoUrl,
                    EmployeeCount = 0 // Start with 0 employees
                };

                var createdCompany = await _companyRepository.InsertAsync(company, autoSave: true);

                // Now create the user
                var user = new Volo.Abp.Identity.IdentityUser(
                    id: Guid.NewGuid(),
                    userName: UserRegistration.Username,
                    email: UserRegistration.Email,
                    tenantId: null);

                user.Name = UserRegistration.Username;
                user.SetEmailConfirmed(true); // Auto-confirm email for now

                var userCreateResult = await _userManager.CreateAsync(user, UserRegistration.Password);

                if (!userCreateResult.Succeeded)
                {
                    var userErrors = string.Join(", ", userCreateResult.Errors.Select(e => e.Description));
                    
                    // Delete the company if user creation failed
                    await _companyRepository.DeleteAsync(createdCompany);
                    
                    return new JsonResult(new { 
                        success = false, 
                        error = $"User creation failed: {userErrors}"
                    });
                }

                // Add a claim to link the user to their company
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("CompanyId", createdCompany.Id.ToString()));
                await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("UserType", "CompanyUser"));

                // Return success response
                return new JsonResult(new { 
                    success = true, 
                    companyId = createdCompany.Id,
                    userId = user.Id,
                    username = UserRegistration.Username,
                    email = UserRegistration.Email,
                    message = "Company and user created successfully! You can now login.",
                    redirectUrl = "/Account/Login"
                });
            }
            catch (Exception ex)
            {
                // Log the detailed error information
                var errorMessage = ex.Message;
                var innerException = ex.InnerException?.Message;
                
                // If it's a validation exception, get the detailed errors
                if (ex is Volo.Abp.Validation.AbpValidationException validationEx)
                {
                    var validationErrors = string.Join("; ", validationEx.ValidationErrors.Select(e => e.ErrorMessage));
                    errorMessage = $"Validation failed: {validationErrors}";
                }
                
                return new JsonResult(new { 
                    success = false, 
                    error = errorMessage,
                    innerError = innerException,
                    fullError = ex.ToString() // For debugging
                });
            }
        }

        private async Task<string> GenerateCompanyCodeAsync(string companyName)
        {
            // Generate code from company name
            var code = new string(companyName.Where(char.IsLetter).Take(3).ToArray()).ToUpper();
            
            // Add number suffix if code already exists
            var counter = 1;
            var baseCode = code;
            
            while (await _companyRepository.AnyAsync(c => c.Code == code))
            {
                code = $"{baseCode}{counter:D2}";
                counter++;
            }
            
            return code;
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