using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            return Task.CompletedTask;
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
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

                return RedirectToPage("/Companies/Index", new { successMessage = "Employee created successfully!" });
            }
            catch (Exception)
            {
                // Log the error and show a user-friendly message
                ModelState.AddModelError("", "An error occurred while creating the employee. Please try again.");
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