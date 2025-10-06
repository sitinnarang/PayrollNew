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

        // Additional fields
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

        [StringLength(100)]
        public string? DisplayName { get; set; }

        [StringLength(20)]
        public string? SocialSecurityNumber { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? BillingRate { get; set; }

        public bool BillableByDefault { get; set; }

        [StringLength(100)]
        public string? Manager { get; set; }

        public string? Gender { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [StringLength(20)]
        public string? MobilePhone { get; set; }

        public string FullName => !string.IsNullOrEmpty(DisplayName) ? DisplayName : $"{FirstName} {LastName}";

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

        public void UpdateExtendedInfo(
            string? address = null,
            string? city = null,
            string? state = null,
            string? zipCode = null,
            string? country = null,
            DateTime? dateOfBirth = null,
            string? emergencyContactName = null,
            string? emergencyContactPhone = null,
            string? displayName = null,
            string? socialSecurityNumber = null,
            decimal? billingRate = null,
            bool billableByDefault = false,
            string? manager = null,
            string? gender = null,
            DateTime? releaseDate = null,
            string? mobilePhone = null)
        {
            Address = address;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            DateOfBirth = dateOfBirth;
            EmergencyContactName = emergencyContactName;
            EmergencyContactPhone = emergencyContactPhone;
            DisplayName = displayName;
            SocialSecurityNumber = socialSecurityNumber;
            BillingRate = billingRate;
            BillableByDefault = billableByDefault;
            Manager = manager;
            Gender = gender;
            ReleaseDate = releaseDate;
            MobilePhone = mobilePhone;
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