using System;
using System.ComponentModel.DataAnnotations;
using PayrollPro.Employees;

namespace PayrollPro.Employees
{
    public class CreateUpdateEmployeeDto
    {
        [Required]
        [CanadianName]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [CanadianName]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [CanadianPhoneNumber]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Required]
        [CanadianEmployeeId]
        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Position")]
        public string Position { get; set; } = string.Empty;

        [Required]
        [CanadianSalary]
        [Display(Name = "Annual Salary (CAD)")]
        public decimal Salary { get; set; }

        [Required]
        [Display(Name = "Hire Date")]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        [Display(Name = "Status")]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [StringLength(1000)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Required]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }

        // Extended Canadian Employee Information
        [StringLength(200, MinimumLength = 5)]
        [Display(Name = "Street Address")]
        public string? Address { get; set; }

        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "City")]
        public string? City { get; set; }

        [CanadianProvince]
        [Display(Name = "Province/Territory")]
        public string? State { get; set; }

        [CanadianPostalCode]
        [Display(Name = "Postal Code")]
        public string? ZipCode { get; set; }

        [StringLength(50)]
        [Display(Name = "Country")]
        public string? Country { get; set; } = "Canada";

        [CanadianEmploymentAge]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [CanadianName]
        [Display(Name = "Emergency Contact Name")]
        public string? EmergencyContactName { get; set; }

        [CanadianPhoneNumber]
        [Display(Name = "Emergency Contact Phone")]
        public string? EmergencyContactPhone { get; set; }

        [CanadianName]
        [Display(Name = "Preferred Name")]
        public string? DisplayName { get; set; }

        [CanadianSIN]
        [Display(Name = "Social Insurance Number")]
        public string? SocialSecurityNumber { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Hourly Billing Rate (CAD)")]
        public decimal? BillingRate { get; set; }

        [Display(Name = "Billable by Default")]
        public bool BillableByDefault { get; set; }

        [CanadianName]
        [Display(Name = "Manager/Supervisor")]
        public string? Manager { get; set; }

        [StringLength(30)]
        [Display(Name = "Gender")]
        [RegularExpression(@"^(Male|Female|Non-binary|Prefer not to say|Other|)$", 
            ErrorMessage = "Please select a valid gender option")]
        public string? Gender { get; set; }

        [Display(Name = "Employment End Date")]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [CanadianPhoneNumber]
        [Display(Name = "Mobile Phone")]
        public string? MobilePhone { get; set; }
    }
}