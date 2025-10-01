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
    }
}