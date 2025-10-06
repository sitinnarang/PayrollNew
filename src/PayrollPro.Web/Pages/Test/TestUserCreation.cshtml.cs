using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace PayrollPro.Web.Pages.Test
{
    public class TestUserCreationModel : PageModel
    {
        private readonly IIdentityUserAppService _identityUserAppService;

        public TestUserCreationModel(IIdentityUserAppService identityUserAppService)
        {
            _identityUserAppService = identityUserAppService;
        }

        [BindProperty]
        [Required]
        public string TestUsername { get; set; } = "testuser123";

        [BindProperty]
        [Required]
        [EmailAddress]
        public string TestEmail { get; set; } = "test@example.com";

        [BindProperty]
        [Required]
        public string TestPassword { get; set; } = "Test123!";

        public TestResultDto? TestResult { get; set; }

        public void OnGet()
        {
            // Nothing to do here
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var userCreateDto = new IdentityUserCreateDto
                {
                    UserName = TestUsername,
                    Email = TestEmail,
                    Password = TestPassword,
                    IsActive = true,
                    LockoutEnabled = false
                };

                var createdUser = await _identityUserAppService.CreateAsync(userCreateDto);

                TestResult = new TestResultDto
                {
                    Success = true,
                    UserId = createdUser.Id.ToString(),
                    Username = createdUser.UserName,
                    Email = createdUser.Email
                };
            }
            catch (Exception ex)
            {
                TestResult = new TestResultDto
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    DetailedError = ex.ToString()
                };

                // If it's a validation exception, get the detailed errors
                if (ex is Volo.Abp.Validation.AbpValidationException validationEx)
                {
                    var validationErrors = string.Join("; ", validationEx.ValidationErrors.Select(e => e.ErrorMessage));
                    TestResult.ErrorMessage = $"Validation failed: {validationErrors}";
                }
            }

            return Page();
        }
    }

    public class TestResultDto
    {
        public bool Success { get; set; }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? ErrorMessage { get; set; }
        public string? DetailedError { get; set; }
    }
}