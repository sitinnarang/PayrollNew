using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace PayrollPro.Companies
{
    public class Company : FullAuditedEntity<Guid>
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string State { get; set; }

        [StringLength(20)]
        public string ZipCode { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(200)]
        public string Website { get; set; }

        [StringLength(50)]
        public string TaxId { get; set; }

        [StringLength(50)]
        public string RegistrationNumber { get; set; }

        public DateTime EstablishedDate { get; set; }

        public bool IsActive { get; set; }

        [StringLength(200)]
        public string LogoUrl { get; set; }

        public int EmployeeCount { get; set; }

        // Payroll Settings
        public PayFrequency PayFrequency { get; set; } = PayFrequency.Monthly;
        
        public DateTime? PayPeriodEnd { get; set; }
        
        [Range(1, 60)]
        public int StandardWorkHours { get; set; } = 40;
        
        [Range(1.0, 5.0)]
        public decimal OvertimeRate { get; set; } = 1.5m;
        
        public bool AutoProcessPayroll { get; set; } = false;

        public Company()
        {
            IsActive = true;
            EstablishedDate = DateTime.Now;
            EmployeeCount = 0;
        }

        // Navigation property for employees
        public virtual ICollection<Employees.Employee> Employees { get; set; }
    }
}