using System;
using Volo.Abp.Application.Dtos;

namespace PayrollPro.Employees
{
    public class EmployeeDto : FullAuditedEntityDto<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public EmployeeStatus Status { get; set; }
        public string? Notes { get; set; }
        public string FullName { get; set; } = string.Empty;
        
        // Company information
        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyCode { get; set; } = string.Empty;

        // Additional fields
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? DisplayName { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public decimal? BillingRate { get; set; }
        public bool BillableByDefault { get; set; }
        public string? Manager { get; set; }
        public string? Gender { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? MobilePhone { get; set; }
        
        // Computed properties
        public int YearsOfService => DateTime.Now.Year - HireDate.Year;
        public string FormattedSalary => Salary.ToString("C");
    }
}