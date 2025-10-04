using System;
using System.ComponentModel.DataAnnotations;

namespace PayrollPro.Companies
{
    public class CreateUpdateCompanyDto
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Company Name")]
        public string Name { get; set; }

        [StringLength(100)]
        [Display(Name = "Company Code")]
        public string Code { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(100)]
        [Display(Name = "State/Province")]
        public string State { get; set; }

        [StringLength(20)]
        [Display(Name = "ZIP/Postal Code")]
        public string ZipCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [StringLength(200)]
        [Url]
        [Display(Name = "Website")]
        public string Website { get; set; }

        [StringLength(50)]
        [Display(Name = "Tax ID")]
        public string TaxId { get; set; }

        [StringLength(50)]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Established Date")]
        public DateTime EstablishedDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [StringLength(200)]
        [Display(Name = "Logo URL")]
        public string? LogoUrl { get; set; }

        // Payroll Settings
        [Display(Name = "Pay Frequency")]
        public PayFrequency PayFrequency { get; set; } = PayFrequency.Monthly;
        
        [Display(Name = "Pay Period End")]
        public DateTime? PayPeriodEnd { get; set; }
        
        [Range(1, 60, ErrorMessage = "Standard work hours must be between 1 and 60")]
        [Display(Name = "Standard Work Hours")]
        public int StandardWorkHours { get; set; } = 40;
        
        [Range(1.0, 5.0, ErrorMessage = "Overtime rate must be between 1.0 and 5.0")]
        [Display(Name = "Overtime Rate")]
        public decimal OvertimeRate { get; set; } = 1.5m;
        
        [Display(Name = "Auto Process Payroll")]
        public bool AutoProcessPayroll { get; set; } = false;

        public CreateUpdateCompanyDto()
        {
            IsActive = true;
            EstablishedDate = DateTime.Now;
        }
    }
}