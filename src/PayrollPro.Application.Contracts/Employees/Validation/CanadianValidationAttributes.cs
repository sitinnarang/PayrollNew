using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace PayrollPro.Employees
{
    /// <summary>
    /// Validates Canadian postal codes (format: K1A 0A6 or K1A0A6)
    /// </summary>
    public class CanadianPostalCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Allow null/empty for optional fields
            }

            var postalCode = value.ToString()!.Replace(" ", "").ToUpperInvariant();
            
            // Canadian postal code pattern: Letter-Digit-Letter Digit-Letter-Digit
            var pattern = @"^[ABCEGHJ-NPRSTVXY]\d[ABCEGHJ-NPRSTV-Z]\d[ABCEGHJ-NPRSTV-Z]\d$";
            
            if (!Regex.IsMatch(postalCode, pattern))
            {
                return new ValidationResult("Please enter a valid Canadian postal code (e.g., K1A 0A6).");
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian phone numbers (10 digits, various formats accepted)
    /// </summary>
    public class CanadianPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Allow null/empty for optional fields
            }

            var phoneNumber = Regex.Replace(value.ToString()!, @"[\s\-\(\)\+\.]", "");
            
            // Remove country code if present
            if (phoneNumber.StartsWith("1") && phoneNumber.Length == 11)
            {
                phoneNumber = phoneNumber.Substring(1);
            }

            // Must be exactly 10 digits
            if (phoneNumber.Length != 10 || !phoneNumber.All(char.IsDigit))
            {
                return new ValidationResult("Please enter a valid Canadian phone number (10 digits).");
            }

            // First digit cannot be 0 or 1
            if (phoneNumber[0] == '0' || phoneNumber[0] == '1')
            {
                return new ValidationResult("Canadian phone numbers cannot start with 0 or 1.");
            }

            // Area code cannot start with 0 or 1
            if (phoneNumber[0] == '0' || phoneNumber[0] == '1')
            {
                return new ValidationResult("Invalid Canadian area code.");
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian Social Insurance Number (SIN)
    /// </summary>
    public class CanadianSINAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Allow null/empty for optional fields
            }

            var sin = Regex.Replace(value.ToString()!, @"[\s\-]", "");
            
            // Must be exactly 9 digits
            if (sin.Length != 9 || !sin.All(char.IsDigit))
            {
                return new ValidationResult("Social Insurance Number must be 9 digits.");
            }

            // Cannot start with 0 or 8
            if (sin[0] == '0' || sin[0] == '8')
            {
                return new ValidationResult("Invalid Social Insurance Number format.");
            }

            // Validate using Luhn algorithm
            if (!IsValidSINChecksum(sin))
            {
                return new ValidationResult("Invalid Social Insurance Number checksum.");
            }

            return ValidationResult.Success!;
        }

        private bool IsValidSINChecksum(string sin)
        {
            var digits = sin.Select(c => int.Parse(c.ToString())).ToArray();
            
            // Double every second digit
            for (int i = 1; i < 9; i += 2)
            {
                digits[i] *= 2;
                if (digits[i] > 9)
                {
                    digits[i] = digits[i] / 10 + digits[i] % 10;
                }
            }

            var sum = digits.Sum();
            return sum % 10 == 0;
        }
    }

    /// <summary>
    /// Validates Canadian province/territory codes
    /// </summary>
    public class CanadianProvinceAttribute : ValidationAttribute
    {
        private static readonly string[] ValidProvinces = {
            "AB", "BC", "MB", "NB", "NL", "NS", "NT", "NU", "ON", "PE", "QC", "SK", "YT"
        };

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!; // Allow null/empty for optional fields
            }

            var province = value.ToString()!.ToUpperInvariant();
            
            if (!ValidProvinces.Contains(province))
            {
                return new ValidationResult($"Please enter a valid Canadian province/territory code: {string.Join(", ", ValidProvinces)}");
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates age for Canadian employment standards (minimum 14 in most provinces)
    /// </summary>
    public class CanadianEmploymentAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public CanadianEmploymentAgeAttribute(int minimumAge = 14)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!; // Allow null for optional fields
            }

            if (value is DateTime dateOfBirth)
            {
                var age = DateTime.Today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                {
                    age--;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult($"Employee must be at least {_minimumAge} years old as per Canadian employment standards.");
                }

                if (age > 100)
                {
                    return new ValidationResult("Please verify the date of birth.");
                }
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian salary ranges and minimum wage compliance
    /// </summary>
    public class CanadianSalaryAttribute : ValidationAttribute
    {
        private readonly decimal _minimumWage;

        public CanadianSalaryAttribute(double minimumWage = 15.00) // Federal minimum wage
        {
            _minimumWage = (decimal)minimumWage;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!;
            }

            if (value is decimal salary)
            {
                // Convert to hourly rate assuming 40 hours/week, 52 weeks/year
                var annualHours = 40 * 52;
                var hourlyRate = salary / annualHours;

                if (hourlyRate < _minimumWage)
                {
                    return new ValidationResult($"Salary must meet Canadian minimum wage requirements (${_minimumWage:F2}/hour).");
                }

                if (salary > 500000) // Reasonable upper limit
                {
                    return new ValidationResult("Please verify the salary amount.");
                }
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates Canadian name formats (handles French-Canadian names)
    /// </summary>
    public class CanadianNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            // Allow null or empty for optional fields - let [Required] handle requirement validation
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success!;
            }

            var name = value.ToString()!.Trim();
            
            // Allow letters, spaces, hyphens, apostrophes (for French-Canadian names)
            var pattern = @"^[a-zA-ZÀ-ÿ\s\-'\.]+$";
            
            if (!Regex.IsMatch(name, pattern))
            {
                return new ValidationResult("Name can only contain letters, spaces, hyphens, and apostrophes.");
            }

            if (name.Length < 2)
            {
                return new ValidationResult("Name must be at least 2 characters long.");
            }

            if (name.Length > 50)
            {
                return new ValidationResult("Name cannot exceed 50 characters.");
            }

            return ValidationResult.Success!;
        }
    }

    /// <summary>
    /// Validates employee ID format for Canadian organizations
    /// </summary>
    public class CanadianEmployeeIdAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult("Employee ID is required.");
            }

            var employeeId = value.ToString()!.Trim();
            
            // Allow alphanumeric characters and common separators
            var pattern = @"^[a-zA-Z0-9\-_]+$";
            
            if (!Regex.IsMatch(employeeId, pattern))
            {
                return new ValidationResult("Employee ID can only contain letters, numbers, hyphens, and underscores.");
            }

            if (employeeId.Length < 3)
            {
                return new ValidationResult("Employee ID must be at least 3 characters long.");
            }

            if (employeeId.Length > 20)
            {
                return new ValidationResult("Employee ID cannot exceed 20 characters.");
            }

            return ValidationResult.Success!;
        }
    }
}