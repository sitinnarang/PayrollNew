using System;
using System.ComponentModel.DataAnnotations;
using PayrollPro.Employees;

namespace PayrollPro.Employees
{
    public class CreateUpdateEmployeeDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(10)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Position { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Salary { get; set; }

        [Required]
        public DateTime HireDate { get; set; }

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}