using System;
using System.ComponentModel.DataAnnotations;
using PayrollPro.Employees; // For shared validation attributes

namespace PayrollPro.Companies
{
    public class CreateUpdateCompanyDto
    {
        [Required]
        [CanadianBusinessName]
        [Display(Name = "Company Name")]
        public string Name { get; set; } = string.Empty;

        [StringLength(20, MinimumLength = 2)]
        [RegularExpression(@"^[A-Z0-9\-_]+$", ErrorMessage = "Company code must contain only uppercase letters, numbers, hyphens, and underscores")]
        [Display(Name = "Company Code")]
        public string? Code { get; set; }

        [StringLength(1000)]
        [Display(Name = "Company Description")]
        public string? Description { get; set; }

        [Required]
        [CanadianBusinessAddress]
        [Display(Name = "Business Address")]
        public string Address { get; set; } = string.Empty;

        [CanadianBusinessCity]
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

        [CanadianPhoneNumber]
        [Display(Name = "Business Phone")]
        public string? Phone { get; set; }

        [CanadianBusinessEmail]
        [Display(Name = "Business Email")]
        public string? Email { get; set; }

        [CanadianBusinessWebsite]
        [Display(Name = "Company Website")]
        public string? Website { get; set; }

        [CanadianTaxId]
        [Display(Name = "Business Number (BN) / Tax ID")]
        public string? TaxId { get; set; }

        [CanadianBusinessNumber]
        [Display(Name = "Corporate Registration Number")]
        public string? RegistrationNumber { get; set; }

        [Required]
        [CanadianBusinessDate]
        [Display(Name = "Date Established")]
        [DataType(DataType.Date)]
        public DateTime EstablishedDate { get; set; }

        [Display(Name = "Company Active")]
        public bool IsActive { get; set; }

        [CanadianBusinessWebsite]
        [Display(Name = "Company Logo URL")]
        public string? LogoUrl { get; set; }

        // Canadian Payroll Settings
        [Display(Name = "Pay Frequency")]
        public PayFrequency PayFrequency { get; set; } = PayFrequency.BiWeekly;
        
        [Display(Name = "Pay Period End Date")]
        [DataType(DataType.Date)]
        public DateTime? PayPeriodEnd { get; set; }
        
        [CanadianPayrollStandards(1, 84)]
        [Display(Name = "Standard Work Hours per Week")]
        public int StandardWorkHours { get; set; } = 40;
        
        [CanadianOvertimeRate]
        [Display(Name = "Overtime Rate Multiplier")]
        public decimal OvertimeRate { get; set; } = 1.5m;
        
        [Display(Name = "Automatically Process Payroll")]
        public bool AutoProcessPayroll { get; set; } = false;

        public CreateUpdateCompanyDto()
        {
            IsActive = true;
            EstablishedDate = DateTime.Now;
        }
    }
}