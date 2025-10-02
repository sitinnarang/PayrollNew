using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace PayrollPro.Employees
{
    public class Employee : FullAuditedAggregateRoot<Guid>
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

        [Required]
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Company relationship
        [Required]
        public Guid CompanyId { get; set; }
        
        public virtual Companies.Company? Company { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        protected Employee()
        {
        }

        public Employee(
            Guid id,
            string firstName,
            string lastName,
            string email,
            string employeeId,
            string department,
            string position,
            decimal salary,
            DateTime hireDate,
            Guid companyId,
            string? phone = null,
            EmployeeStatus status = EmployeeStatus.Active,
            string? notes = null) : base(id)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            EmployeeId = employeeId;
            Department = department;
            Position = position;
            Salary = salary;
            HireDate = hireDate;
            Status = status;
            Notes = notes;
            CompanyId = companyId;
        }

        public void UpdatePersonalInfo(string firstName, string lastName, string email, string? phone = null)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
        }

        public void UpdateJobInfo(string department, string position, decimal salary, Guid companyId)
        {
            Department = department;
            Position = position;
            Salary = salary;
            CompanyId = companyId;
        }

        public void UpdateStatus(EmployeeStatus status)
        {
            Status = status;
        }
    }
}