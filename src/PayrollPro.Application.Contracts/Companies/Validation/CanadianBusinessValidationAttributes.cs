using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace PayrollPro.Companies
{
    /// <summary>
    /// Validates Canadian business names according to federal and provincial regulations
    /// </summary>
    public class CanadianBusinessNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Company name is required.");
            }

            var businessName = value.ToString()!.Trim();
            
            // Length validation
            if (businessName.Length < 2)
            {
                return new ValidationResult("Company name must be at least 2 characters long.");
            }

            if (businessName.Length > 200)
            {
                return new ValidationResult("Company name cannot exceed 200 characters.");
            }

            // Canadian business name pattern - allows letters, numbers, spaces, and common business punctuation
            var pattern = @"^[a-zA-ZÀ-ÿ0-9\s\-'&\.(),/]+$";
            
            if (!Regex.IsMatch(businessName, pattern))
            {
                return new ValidationResult("Company name contains invalid characters. Only letters, numbers, spaces, hyphens, apostrophes, ampersands, periods, commas, parentheses, and forward slashes are allowed.");
            }

            // Check for prohibited terms (basic validation)
            var prohibitedTerms = new[] { "BANK", "INSURANCE", "TRUST" };
            var upperName = businessName.ToUpperInvariant();
            
            foreach (var term in prohibitedTerms)
            {
                if (upperName.Contains(term) && !upperName.Contains("BANKING") && !upperName.Contains("CONSULTING"))
                {
                    return new ValidationResult($"Company names containing '{term}' may require special licensing. Please verify regulatory compliance.");
                }
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian business registration numbers (BN/GST/HST)
    /// </summary>
    public class CanadianBusinessNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Optional field
            }

            var businessNumber = Regex.Replace(value.ToString()!, @"[\s\-]", "");
            
            // Canadian Business Number format: 9 digits + 2 letter program identifier + 4 digit reference number
            // Example: 123456789RC0001 (BN9 + RC + 0001)
            var bnPattern = @"^\d{9}[A-Z]{2}\d{4}$";
            
            if (businessNumber.Length == 9 && businessNumber.All(char.IsDigit))
            {
                // Just the 9-digit BN number
                return ValidationResult.Success!;
            }
            
            if (businessNumber.Length == 15 && Regex.IsMatch(businessNumber, bnPattern))
            {
                // Full BN with program identifier
                return ValidationResult.Success!;
            }

            return new ValidationResult("Please enter a valid Canadian Business Number (9 digits) or full BN with program identifier (15 characters).");
        }
    }

    /// <summary>
    /// Validates Canadian corporate tax identification numbers
    /// </summary>
    public class CanadianTaxIdAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Optional field
            }

            var taxId = Regex.Replace(value.ToString()!, @"[\s\-]", "");
            
            // Canadian corporate tax IDs can be:
            // 1. Business Number (9 digits)
            // 2. GST/HST number (BN + RT program identifier)
            // 3. Payroll program number (BN + RP program identifier)
            
            if (taxId.Length == 9 && taxId.All(char.IsDigit))
            {
                return ValidationResult.Success!;
            }
            
            if (taxId.Length == 15)
            {
                var bnPart = taxId.Substring(0, 9);
                var programId = taxId.Substring(9, 2);
                var refNumber = taxId.Substring(11, 4);
                
                if (bnPart.All(char.IsDigit) && 
                    (programId == "RT" || programId == "RP" || programId == "RC" || programId == "RM") &&
                    refNumber.All(char.IsDigit))
                {
                    return ValidationResult.Success!;
                }
            }

            return new ValidationResult("Please enter a valid Canadian Tax ID (Business Number format).");
        }
    }

    /// <summary>
    /// Validates Canadian business establishment dates
    /// </summary>
    public class CanadianBusinessDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Establishment date is required.");
            }

            if (value is DateTime establishedDate)
            {
                var today = DateTime.Today;
                
                // Cannot be in the future
                if (establishedDate.Date > today)
                {
                    return new ValidationResult("Establishment date cannot be in the future.");
                }
                
                // Cannot be more than 200 years ago (reasonable business limit)
                if (establishedDate.Date < today.AddYears(-200))
                {
                    return new ValidationResult("Please verify the establishment date.");
                }
                
                // Special validation for Canadian Confederation (1867)
                if (establishedDate.Date < new DateTime(1867, 7, 1))
                {
                    return new ValidationResult("Establishment date should not predate Canadian Confederation (July 1, 1867) unless for historical entities.");
                }
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian business website URLs
    /// </summary>
    public class CanadianBusinessWebsiteAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Optional field
            }

            var website = value.ToString()!.Trim();
            
            // Basic URL validation
            if (!Uri.TryCreate(website, UriKind.Absolute, out var uri))
            {
                return new ValidationResult("Please enter a valid website URL (e.g., https://www.example.com).");
            }

            // Must use HTTP or HTTPS
            if (uri.Scheme != "http" && uri.Scheme != "https")
            {
                return new ValidationResult("Website URL must use HTTP or HTTPS protocol.");
            }

            // Length validation
            if (website.Length > 200)
            {
                return new ValidationResult("Website URL cannot exceed 200 characters.");
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian business email addresses
    /// </summary>
    public class CanadianBusinessEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Optional field
            }

            var email = value.ToString()!.Trim();
            
            // Basic email validation
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                return new ValidationResult("Please enter a valid email address.");
            }

            // Length validation
            if (email.Length > 100)
            {
                return new ValidationResult("Email address cannot exceed 100 characters.");
            }

            // Business email validation (not personal email providers for business registration)
            var personalProviders = new[] { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "live.com" };
            var domain = email.Split('@').LastOrDefault()?.ToLowerInvariant();
            
            if (personalProviders.Contains(domain))
            {
                return new ValidationResult("Consider using a business email address for company registration (personal email providers detected).");
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian business addresses
    /// </summary>
    public class CanadianBusinessAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Business address is required for Canadian companies.");
            }

            var address = value.ToString()!.Trim();
            
            // Length validation
            if (address.Length < 10)
            {
                return new ValidationResult("Please provide a complete business address (minimum 10 characters).");
            }

            if (address.Length > 500)
            {
                return new ValidationResult("Business address cannot exceed 500 characters.");
            }

            // Basic address pattern validation
            var addressPattern = @"^[a-zA-ZÀ-ÿ0-9\s\-'#.,/()]+$";
            
            if (!Regex.IsMatch(address, addressPattern))
            {
                return new ValidationResult("Business address contains invalid characters.");
            }

            // Should contain at least one number (street number)
            if (!Regex.IsMatch(address, @"\d"))
            {
                return new ValidationResult("Please include a street number in the business address.");
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian business cities
    /// </summary>
    public class CanadianBusinessCityAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Optional but recommended
            }

            var city = value.ToString()!.Trim();
            
            // Length validation
            if (city.Length < 2)
            {
                return new ValidationResult("City name must be at least 2 characters long.");
            }

            if (city.Length > 100)
            {
                return new ValidationResult("City name cannot exceed 100 characters.");
            }

            // Canadian city name pattern (supports French-Canadian names)
            var cityPattern = @"^[a-zA-ZÀ-ÿ\s\-'\.]+$";
            
            if (!Regex.IsMatch(city, cityPattern))
            {
                return new ValidationResult("City name can only contain letters, spaces, hyphens, apostrophes, and periods.");
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian payroll standards for businesses
    /// </summary>
    public class CanadianPayrollStandardsAttribute : ValidationAttribute
    {
        private readonly int _minHours;
        private readonly int _maxHours;

        public CanadianPayrollStandardsAttribute(int minHours = 1, int maxHours = 84)
        {
            _minHours = minHours;
            _maxHours = maxHours;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            if (value is int hours)
            {
                if (hours < _minHours)
                {
                    return new ValidationResult($"Standard work hours must be at least {_minHours} hour per week.");
                }

                if (hours > _maxHours)
                {
                    return new ValidationResult($"Standard work hours cannot exceed {_maxHours} hours per week as per Canadian employment standards.");
                }

                // Warning for excessive hours
                if (hours > 60)
                {
                    return new ValidationResult("Work hours over 60 per week may require special employment agreements and compliance with provincial overtime regulations.");
                }
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian overtime rates according to employment standards
    /// </summary>
    public class CanadianOvertimeRateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            if (value is decimal rate)
            {
                // Canadian federal minimum overtime rate is 1.5x
                if (rate < 1.5m)
                {
                    return new ValidationResult("Canadian employment standards require minimum 1.5x overtime rate for hours worked over standard hours.");
                }

                // Maximum reasonable overtime rate
                if (rate > 3.0m)
                {
                    return new ValidationResult("Overtime rate exceeds typical Canadian employment standards (maximum 3.0x recommended).");
                }
            }

            return ValidationResult.Success!;
        }
    }
}