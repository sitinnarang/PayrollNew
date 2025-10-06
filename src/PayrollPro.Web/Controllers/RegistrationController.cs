using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Identity;
using PayrollPro.Companies;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PayrollPro.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Security.Claims;
using System;
using System.Linq;

namespace PayrollPro.Web.Controllers
{
    [AllowAnonymous]
    [Route("api/registration")]
    public class RegistrationController : Controller
    {
        private readonly UserManager<Volo.Abp.Identity.IdentityUser> _userManager;
        private readonly PayrollProDbContext _dbContext;

        public RegistrationController(
            UserManager<Volo.Abp.Identity.IdentityUser> userManager,
            PayrollProDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [HttpPost("company-and-user")]
        public async Task<IActionResult> CreateCompanyAndUser([FromBody] CompanyUserRegistrationRequest request)
        {
            try
            {
                // Validate the request
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    return BadRequest(new { success = false, error = errors });
                }

                // Create user first
                var newUser = new Volo.Abp.Identity.IdentityUser(
                    Guid.NewGuid(),
                    request.Username,
                    request.Email,
                    null // tenantId
                )
                {
                    Name = "User",
                    Surname = "User"
                };

                var userResult = await _userManager.CreateAsync(newUser, request.Password);
                if (!userResult.Succeeded)
                {
                    var errors = string.Join(", ", userResult.Errors.Select(e => e.Description));
                    return BadRequest(new { success = false, error = $"User creation failed: {errors}" });
                }

                // Log successful user creation for debugging
                Console.WriteLine($"User created successfully: {newUser.Email}, ID: {newUser.Id}");

                // Confirm email immediately
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                await _userManager.ConfirmEmailAsync(newUser, token);

                // Create company directly in database
                var company = new Company
                {
                    Name = request.CompanyName,
                    Code = GenerateCompanyCode(request.CompanyName),
                    Description = request.Description,
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    ZipCode = request.ZipCode,
                    Country = request.Country,
                    Phone = request.Phone,
                    Email = request.CompanyEmail,
                    Website = request.Website,
                    TaxId = request.TaxId,
                    RegistrationNumber = request.RegistrationNumber,
                    EstablishedDate = request.EstablishedDate,
                    IsActive = true,
                    LogoUrl = null,
                    EmployeeCount = 0
                };

                _dbContext.Companies.Add(company);
                await _dbContext.SaveChangesAsync();

                // Add company claim to user
                var companyClaim = new Claim("CompanyId", company.Id.ToString());
                await _userManager.AddClaimAsync(newUser, companyClaim);

                return Ok(new
                {
                    success = true,
                    companyId = company.Id,
                    userId = newUser.Id,
                    username = request.Username,
                    email = request.Email,
                    message = "Company and user account created successfully! You can now login with your credentials."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        [HttpPost("test-password")]
        public async Task<IActionResult> TestPassword([FromBody] TestPasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return NotFound(new { success = false, error = "User not found" });
                }

                var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                var isLockedOut = await _userManager.IsLockedOutAsync(user);
                var failedAttempts = await _userManager.GetAccessFailedCountAsync(user);

                return Ok(new
                {
                    success = true,
                    isPasswordValid = isPasswordValid,
                    isLockedOut = isLockedOut,
                    failedAttempts = failedAttempts,
                    userId = user.Id,
                    email = user.Email,
                    emailConfirmed = user.EmailConfirmed,
                    isActive = user.IsActive
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, error = ex.Message });
            }
        }

        private string GenerateCompanyCode(string companyName)
        {
            // Simple code generation - just take first 3 letters and add random number
            var code = new string(companyName.Where(char.IsLetter).Take(3).ToArray()).ToUpper();
            var random = new Random();
            return $"{code}{random.Next(10, 99)}";
        }
    }

    public class CompanyUserRegistrationRequest
    {
        [Required]
        public string CompanyName { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string Address { get; set; } = string.Empty;
        
        public string City { get; set; } = string.Empty;
        
        public string State { get; set; } = string.Empty;
        
        public string ZipCode { get; set; } = string.Empty;
        
        public string Country { get; set; } = string.Empty;
        
        public string Phone { get; set; } = string.Empty;
        
        public string CompanyEmail { get; set; } = string.Empty;
        
        public string Website { get; set; } = string.Empty;
        
        public string TaxId { get; set; } = string.Empty;
        
        public string RegistrationNumber { get; set; } = string.Empty;
        
        public DateTime EstablishedDate { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class TestPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}