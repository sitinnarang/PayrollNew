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
    }
}