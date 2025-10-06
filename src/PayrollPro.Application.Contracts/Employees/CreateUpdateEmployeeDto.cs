using System;
using System.ComponentModel.DataAnnotations;
using PayrollPro.Employees;

namespace PayrollPro.Employees
{
    public class CreateUpdateEmployeeDto
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "Employee ID")]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Department")]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Position")]
        public string Position { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }

        [Required]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        [Display(Name = "Status")]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [Required]
        [Display(Name = "Company")]
        public Guid CompanyId { get; set; }

        // Additional fields
        [StringLength(200)]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(50)]
        [Display(Name = "City")]
        public string? City { get; set; }

        [StringLength(50)]
        [Display(Name = "State")]
        public string? State { get; set; }

        [StringLength(20)]
        [Display(Name = "Zip Code")]
        public string? ZipCode { get; set; }

        [StringLength(50)]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(100)]
        [Display(Name = "Emergency Contact Name")]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        [Display(Name = "Emergency Contact Phone")]
        public string? EmergencyContactPhone { get; set; }

        [StringLength(100)]
        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }

        [StringLength(20)]
        [Display(Name = "Social Security Number")]
        public string? SocialSecurityNumber { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Billing Rate")]
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
    }
}