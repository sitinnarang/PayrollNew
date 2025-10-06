using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PayrollPro.Employees;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace PayrollPro.Web.Pages.Employees
{
    public class CreateModel : AbpPageModel
    {
        [BindProperty]
        public CreateEmployeeViewModel Employee { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid? CompanyId { get; set; }

        private readonly IEmployeeAppService _employeeAppService;

        public CreateModel(IEmployeeAppService employeeAppService)
        {
            _employeeAppService = employeeAppService;
        }

        public virtual Task OnGetAsync()
        {
            Employee = new CreateEmployeeViewModel();
            if (CompanyId.HasValue)
            {
                Employee.CompanyId = CompanyId.Value;
            }
            else
            {
                // Set default company ID if none provided - use first company from database
                Employee.CompanyId = new Guid("5ED98643-37E3-E3D0-83A9-3A1CC380A120"); // TechNova Solutions
            }
            return Task.CompletedTask;
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            // Log form submission attempt
            Logger.LogInformation("Employee creation form submitted. CompanyId: {CompanyId}, FirstName: {FirstName}, LastName: {LastName}", 
                Employee.CompanyId, Employee.FirstName, Employee.LastName);

            // Validate CompanyId is set
            if (Employee.CompanyId == Guid.Empty)
            {
                Logger.LogWarning("CompanyId was empty, setting default to TechNova Solutions");
                Employee.CompanyId = new Guid("5ED98643-37E3-E3D0-83A9-3A1CC380A120"); // Default to TechNova Solutions
            }

            if (!ModelState.IsValid)
            {
                // Add detailed debugging information about validation errors
                Logger.LogError("ModelState validation failed with {ErrorCount} field(s)", ModelState.ErrorCount);
                
                foreach (var modelError in ModelState.Where(x => x.Value?.Errors?.Count > 0))
                {
                    var fieldName = modelError.Key;
                    foreach (var error in modelError.Value!.Errors)
                    {
                        var errorMessage = !string.IsNullOrEmpty(error.ErrorMessage) ? error.ErrorMessage : error.Exception?.Message ?? "Unknown error";
                        Logger.LogError("Field '{FieldName}' has error: '{ErrorMessage}'", fieldName, errorMessage);
                        
                        // If the error message contains placeholders, log it as a template issue
                        if (errorMessage.Contains("{0}") || errorMessage.Contains("{") && errorMessage.Contains("}"))
                        {
                            Logger.LogError("Error message template issue detected for field '{FieldName}': '{ErrorTemplate}'", fieldName, errorMessage);
                        }
                    }
                }
                
                var allErrors = ModelState.Where(x => x.Value?.Errors?.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => !string.IsNullOrEmpty(x.ErrorMessage) ? x.ErrorMessage : x.Exception?.Message ?? "Unknown error")
                    .Where(x => !string.IsNullOrEmpty(x));
                
                ModelState.AddModelError("", $"Please fix the following validation errors and try again.");
                return Page();
            }

            try
            {
                var input = new CreateUpdateEmployeeDto
                {
                    FirstName = Employee.FirstName,
                    LastName = Employee.LastName,
                    Email = Employee.Email,
                    Phone = Employee.Phone,
                    EmployeeId = Employee.EmployeeId,
                    Department = Employee.Department,
                    Position = Employee.Position,
                    HireDate = Employee.HireDate,
                    Salary = Employee.Salary,
                    Status = Employee.Status,
                    CompanyId = Employee.CompanyId,
                    Notes = Employee.Notes,
                    Address = Employee.Address,
                    City = Employee.City,
                    State = Employee.State,
                    ZipCode = Employee.ZipCode,
                    Country = Employee.Country,
                    DateOfBirth = Employee.DateOfBirth,
                    EmergencyContactName = Employee.EmergencyContactName,
                    EmergencyContactPhone = Employee.EmergencyContactPhone,
                    DisplayName = Employee.DisplayName,
                    SocialSecurityNumber = Employee.SocialSecurityNumber,
                    BillingRate = Employee.BillingRate,
                    BillableByDefault = Employee.BillableByDefault,
                    Manager = Employee.Manager,
                    Gender = Employee.Gender,
                    ReleaseDate = Employee.ReleaseDate,
                    MobilePhone = Employee.MobilePhone
                };

                await _employeeAppService.CreateAsync(input);

                // Set success message in TempData
                TempData["SuccessMessage"] = $"Employee '{Employee.FirstName} {Employee.LastName}' created successfully!";
                TempData["SuccessTitle"] = "Employee Created";
                
                Logger.LogInformation("Employee '{FirstName} {LastName}' created successfully for CompanyId: {CompanyId}", 
                    Employee.FirstName, Employee.LastName, Employee.CompanyId);

                return RedirectToPage("/Companies/Details", new { id = Employee.CompanyId });
            }
            catch (Exception ex)
            {
                // Log the error and show a user-friendly message
                Logger.LogError(ex, "Error creating employee: {ErrorMessage}", ex.Message);
                
                // Check if it's an authorization exception
                if (ex is Volo.Abp.Authorization.AbpAuthorizationException)
                {
                    TempData["ErrorMessage"] = "You don't have permission to create employees. Please contact your administrator to grant you the 'Employee Creation' permission.";
                    TempData["ErrorTitle"] = "Permission Denied";
                    Logger.LogError("Authorization failed for employee creation. User lacks PayrollPro.Employees.Create permission.");
                }
                // Check if it's a validation exception and add specific validation errors
                else if (ex is Volo.Abp.Validation.AbpValidationException validationEx)
                {
                    Logger.LogError("ABP Validation Exception occurred with {ValidationErrorCount} errors", validationEx.ValidationErrors.Count);
                    
                    var errorMessages = new List<string>();
                    foreach (var validationResult in validationEx.ValidationErrors)
                    {
                        var memberName = validationResult.MemberNames?.FirstOrDefault() ?? "Unknown";
                        var errorMessage = validationResult.ErrorMessage ?? "Validation failed";
                        
                        ModelState.AddModelError(memberName, errorMessage);
                        errorMessages.Add($"{memberName}: {errorMessage}");
                        Logger.LogError("Validation error for {MemberName}: {ErrorMessage}", memberName, errorMessage);
                    }
                    
                    TempData["ErrorMessage"] = "Validation errors occurred:\n" + string.Join("\n", errorMessages);
                    TempData["ErrorTitle"] = "Validation Failed";
                    
                    // Log the full validation exception details
                    Logger.LogError("Full validation exception: {@ValidationException}", validationEx);
                }
                else
                {
                    // Generic error handling
                    var userFriendlyMessage = ex.Message.Contains("duplicate") 
                        ? "An employee with this information already exists. Please check Employee ID and Email." 
                        : "An unexpected error occurred while creating the employee. Please check your information and try again.";
                        
                    TempData["ErrorMessage"] = userFriendlyMessage;
                    TempData["ErrorTitle"] = "Error Creating Employee";
                    TempData["ErrorDetails"] = ex.Message; // For debugging purposes
                    
                    ModelState.AddModelError("", userFriendlyMessage);
                }
                
                return Page();
            }
        }
    }

    public class CreateEmployeeViewModel
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(20)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Position { get; set; } = string.Empty;

        [Required]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        [Required]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [Required]
        public Guid CompanyId { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? ZipCode { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }

        // Additional professional fields
        [StringLength(100)]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [StringLength(20)]
        [Display(Name = "Social Security No.")]
        public string? SocialSecurityNumber { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Billing Rate ($/hr)")]
        public decimal? BillingRate { get; set; }

        [Display(Name = "Billable by Default")]
        public bool BillableByDefault { get; set; }

        [StringLength(100)]
        [Display(Name = "Manager")]
        public string? Manager { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }

        [StringLength(20)]
        [Display(Name = "Mobile Phone")]
        public string? MobilePhone { get; set; }

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
    }
}