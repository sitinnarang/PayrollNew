// Canadian Business Validation Script
// Provides comprehensive client-side validation for Canadian business registration standards

(function() {
    'use strict';
    
    // Canadian business validation patterns
    const CANADIAN_BUSINESS_PATTERNS = {
        businessNumber: /^\d{9}([A-Z]{2}\d{4})?$/,
        businessName: /^[a-zA-ZÀ-ÿ0-9\s\-'&\.(),/]+$/,
        businessCode: /^[A-Z0-9\-_]+$/,
        businessEmail: /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/,
        businessWebsite: /^https?:\/\/[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}(\/.*)?$/
    };
    
    // Canadian provinces/territories
    const CANADIAN_PROVINCES = [
        'AB', 'BC', 'MB', 'NB', 'NL', 'NS', 'NT', 'NU', 'ON', 'PE', 'QC', 'SK', 'YT'
    ];
    
    // Prohibited business terms that may require licensing
    const PROHIBITED_TERMS = ['BANK', 'INSURANCE', 'TRUST'];
    
    // Personal email providers (business registration should use business emails)
    const PERSONAL_EMAIL_PROVIDERS = ['gmail.com', 'yahoo.com', 'hotmail.com', 'outlook.com', 'live.com'];
    
    // Canadian business standards
    const BUSINESS_STANDARDS = {
        minimumEstablishedDate: new Date('1867-07-01'), // Canadian Confederation
        overtimeMinimum: 1.5,
        overtimeMaximum: 3.0,
        workHoursMinimum: 1,
        workHoursMaximum: 84 // Canadian employment standards
    };
    
    // Validation functions
    function validateCanadianBusinessName(name) {
        if (!name || name.trim().length === 0) {
            return { isValid: false, message: 'Company name is required.' };
        }
        
        const trimmed = name.trim();
        
        if (trimmed.length < 2) {
            return { isValid: false, message: 'Company name must be at least 2 characters long.' };
        }
        
        if (trimmed.length > 200) {
            return { isValid: false, message: 'Company name cannot exceed 200 characters.' };
        }
        
        if (!CANADIAN_BUSINESS_PATTERNS.businessName.test(trimmed)) {
            return { 
                isValid: false, 
                message: 'Company name contains invalid characters. Only letters, numbers, spaces, hyphens, apostrophes, ampersands, periods, commas, parentheses, and forward slashes are allowed.' 
            };
        }
        
        // Check for prohibited terms
        const upperName = trimmed.toUpperCase();
        for (const term of PROHIBITED_TERMS) {
            if (upperName.includes(term) && !upperName.includes(term + 'ING') && !upperName.includes('CONSULTING')) {
                return {
                    isValid: false,
                    message: `Company names containing '${term}' may require special licensing. Please verify regulatory compliance.`
                };
            }
        }
        
        return { isValid: true };
    }
    
    function validateBusinessCode(code) {
        if (!code || code.trim().length === 0) {
            return { isValid: true }; // Optional field
        }
        
        const trimmed = code.trim();
        
        if (trimmed.length < 2) {
            return { isValid: false, message: 'Company code must be at least 2 characters long.' };
        }
        
        if (trimmed.length > 20) {
            return { isValid: false, message: 'Company code cannot exceed 20 characters.' };
        }
        
        if (!CANADIAN_BUSINESS_PATTERNS.businessCode.test(trimmed)) {
            return { 
                isValid: false, 
                message: 'Company code must contain only uppercase letters, numbers, hyphens, and underscores.' 
            };
        }
        
        return { isValid: true };
    }
    
    function validateCanadianBusinessAddress(address) {
        if (!address || address.trim().length === 0) {
            return { isValid: false, message: 'Business address is required for Canadian companies.' };
        }
        
        const trimmed = address.trim();
        
        if (trimmed.length < 10) {
            return { isValid: false, message: 'Please provide a complete business address (minimum 10 characters).' };
        }
        
        if (trimmed.length > 500) {
            return { isValid: false, message: 'Business address cannot exceed 500 characters.' };
        }
        
        // Should contain at least one number (street number)
        if (!/\d/.test(trimmed)) {
            return { isValid: false, message: 'Please include a street number in the business address.' };
        }
        
        return { isValid: true };
    }
    
    function validateCanadianBusinessCity(city) {
        if (!city || city.trim().length === 0) {
            return { isValid: true }; // Optional field
        }
        
        const trimmed = city.trim();
        
        if (trimmed.length < 2) {
            return { isValid: false, message: 'City name must be at least 2 characters long.' };
        }
        
        if (trimmed.length > 100) {
            return { isValid: false, message: 'City name cannot exceed 100 characters.' };
        }
        
        // Canadian city name pattern (supports French-Canadian names)
        const cityPattern = /^[a-zA-ZÀ-ÿ\s\-'\.]+$/;
        
        if (!cityPattern.test(trimmed)) {
            return { 
                isValid: false, 
                message: 'City name can only contain letters, spaces, hyphens, apostrophes, and periods.' 
            };
        }
        
        return { isValid: true };
    }
    
    function validateCanadianBusinessEmail(email) {
        if (!email || email.trim().length === 0) {
            return { isValid: true }; // Optional field
        }
        
        const trimmed = email.trim();
        
        if (!CANADIAN_BUSINESS_PATTERNS.businessEmail.test(trimmed)) {
            return { isValid: false, message: 'Please enter a valid email address.' };
        }
        
        if (trimmed.length > 100) {
            return { isValid: false, message: 'Email address cannot exceed 100 characters.' };
        }
        
        // Check for personal email providers
        const domain = trimmed.split('@')[1]?.toLowerCase();
        if (PERSONAL_EMAIL_PROVIDERS.includes(domain)) {
            return {
                isValid: false,
                message: 'Consider using a business email address for company registration (personal email providers detected).'
            };
        }
        
        return { isValid: true };
    }
    
    function validateCanadianBusinessWebsite(website) {
        if (!website || website.trim().length === 0) {
            return { isValid: true }; // Optional field
        }
        
        const trimmed = website.trim();
        
        if (trimmed.length > 200) {
            return { isValid: false, message: 'Website URL cannot exceed 200 characters.' };
        }
        
        try {
            const url = new URL(trimmed);
            if (url.protocol !== 'http:' && url.protocol !== 'https:') {
                return { isValid: false, message: 'Website URL must use HTTP or HTTPS protocol.' };
            }
        } catch (e) {
            return { isValid: false, message: 'Please enter a valid website URL (e.g., https://www.example.com).' };
        }
        
        return { isValid: true };
    }
    
    function validateCanadianBusinessNumber(businessNumber) {
        if (!businessNumber || businessNumber.trim().length === 0) {
            return { isValid: true }; // Optional field
        }
        
        const cleaned = businessNumber.replace(/[\s\-]/g, '');
        
        if (cleaned.length === 9 && /^\d{9}$/.test(cleaned)) {
            return { isValid: true };
        }
        
        if (cleaned.length === 15 && CANADIAN_BUSINESS_PATTERNS.businessNumber.test(cleaned)) {
            return { isValid: true };
        }
        
        return { 
            isValid: false, 
            message: 'Please enter a valid Canadian Business Number (9 digits) or full BN with program identifier (15 characters).' 
        };
    }
    
    function validateCanadianTaxId(taxId) {
        if (!taxId || taxId.trim().length === 0) {
            return { isValid: true }; // Optional field
        }
        
        const cleaned = taxId.replace(/[\s\-]/g, '');
        
        if (cleaned.length === 9 && /^\d{9}$/.test(cleaned)) {
            return { isValid: true };
        }
        
        if (cleaned.length === 15) {
            const bnPart = cleaned.substring(0, 9);
            const programId = cleaned.substring(9, 11);
            const refNumber = cleaned.substring(11, 15);
            
            if (/^\d{9}$/.test(bnPart) && 
                ['RT', 'RP', 'RC', 'RM'].includes(programId) &&
                /^\d{4}$/.test(refNumber)) {
                return { isValid: true };
            }
        }
        
        return { 
            isValid: false, 
            message: 'Please enter a valid Canadian Tax ID (Business Number format).' 
        };
    }
    
    function validateBusinessEstablishedDate(date) {
        if (!date) {
            return { isValid: false, message: 'Establishment date is required.' };
        }
        
        const establishedDate = new Date(date);
        const today = new Date();
        
        if (establishedDate > today) {
            return { isValid: false, message: 'Establishment date cannot be in the future.' };
        }
        
        if (establishedDate < BUSINESS_STANDARDS.minimumEstablishedDate) {
            return { 
                isValid: false, 
                message: 'Establishment date should not predate Canadian Confederation (July 1, 1867) unless for historical entities.' 
            };
        }
        
        if (establishedDate < new Date(today.getFullYear() - 200, 0, 1)) {
            return { isValid: false, message: 'Please verify the establishment date.' };
        }
        
        return { isValid: true };
    }
    
    function validateCanadianWorkHours(hours) {
        if (!hours || hours === 0) {
            return { isValid: true };
        }
        
        const workHours = parseInt(hours);
        
        if (workHours < BUSINESS_STANDARDS.workHoursMinimum) {
            return { 
                isValid: false, 
                message: `Standard work hours must be at least ${BUSINESS_STANDARDS.workHoursMinimum} hour per week.` 
            };
        }
        
        if (workHours > BUSINESS_STANDARDS.workHoursMaximum) {
            return { 
                isValid: false, 
                message: `Standard work hours cannot exceed ${BUSINESS_STANDARDS.workHoursMaximum} hours per week as per Canadian employment standards.` 
            };
        }
        
        if (workHours > 60) {
            return { 
                isValid: false, 
                message: 'Work hours over 60 per week may require special employment agreements and compliance with provincial overtime regulations.' 
            };
        }
        
        return { isValid: true };
    }
    
    function validateCanadianOvertimeRate(rate) {
        if (!rate || rate === 0) {
            return { isValid: true };
        }
        
        const overtimeRate = parseFloat(rate);
        
        if (overtimeRate < BUSINESS_STANDARDS.overtimeMinimum) {
            return { 
                isValid: false, 
                message: 'Canadian employment standards require minimum 1.5x overtime rate for hours worked over standard hours.' 
            };
        }
        
        if (overtimeRate > BUSINESS_STANDARDS.overtimeMaximum) {
            return { 
                isValid: false, 
                message: 'Overtime rate exceeds typical Canadian employment standards (maximum 3.0x recommended).' 
            };
        }
        
        return { isValid: true };
    }
    
    // Format functions
    function formatBusinessNumber(input) {
        const cleaned = input.replace(/[\s\-]/g, '');
        
        if (cleaned.length === 9) {
            return cleaned.replace(/(\d{3})(\d{3})(\d{3})/, '$1-$2-$3');
        } else if (cleaned.length === 15) {
            return cleaned.replace(/(\d{9})([A-Z]{2})(\d{4})/, '$1-$2-$3');
        }
        
        return input;
    }
    
    function formatBusinessCode(input) {
        return input.toUpperCase().replace(/[^A-Z0-9\-_]/g, '');
    }
    
    // Real-time validation setup
    function setupBusinessFieldValidation() {
        // Company Name validation (ASP.NET Core generates Company_Name as ID)
        const nameField = document.getElementById('Company_Name');
        if (nameField) {
            nameField.addEventListener('blur', function() {
                const result = validateCanadianBusinessName(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Company Code validation and formatting
        const codeField = document.getElementById('Company_Code');
        if (codeField) {
            codeField.addEventListener('input', function() {
                this.value = formatBusinessCode(this.value);
            });
            
            codeField.addEventListener('blur', function() {
                const result = validateBusinessCode(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Business Address validation
        const addressField = document.getElementById('Company_Address');
        if (addressField) {
            addressField.addEventListener('blur', function() {
                const result = validateCanadianBusinessAddress(this.value);
                showValidationResult(this, result);
            });
        }
        
        // City validation
        const cityField = document.getElementById('Company_City');
        if (cityField) {
            cityField.addEventListener('blur', function() {
                const result = validateCanadianBusinessCity(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Province validation (using existing employee validation)
        const provinceField = document.getElementById('Company_State');
        if (provinceField && window.CanadianEmployeeValidation) {
            provinceField.addEventListener('blur', function() {
                const result = window.CanadianEmployeeValidation.validateProvince(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Postal Code validation (using existing employee validation)
        const postalCodeField = document.getElementById('Company_ZipCode');
        if (postalCodeField && window.CanadianEmployeeValidation) {
            postalCodeField.addEventListener('blur', function() {
                const result = window.CanadianEmployeeValidation.validatePostalCode(this.value);
                if (result.isValid && this.value) {
                    this.value = window.CanadianEmployeeValidation.formatPostalCode(this.value);
                }
                showValidationResult(this, result);
            });
        }
        
        // Phone validation (using existing employee validation)
        const phoneField = document.getElementById('Company_Phone');
        if (phoneField && window.CanadianEmployeeValidation) {
            phoneField.addEventListener('blur', function() {
                const result = window.CanadianEmployeeValidation.validatePhoneNumber(this.value);
                if (result.isValid && this.value) {
                    this.value = window.CanadianEmployeeValidation.formatPhoneNumber(this.value);
                }
                showValidationResult(this, result);
            });
        }
        
        // Email validation
        const emailField = document.getElementById('Company_Email');
        if (emailField) {
            emailField.addEventListener('blur', function() {
                const result = validateCanadianBusinessEmail(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Website validation
        const websiteField = document.getElementById('Company_Website');
        if (websiteField) {
            websiteField.addEventListener('blur', function() {
                const result = validateCanadianBusinessWebsite(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Tax ID validation with formatting
        const taxIdField = document.getElementById('Company_TaxId');
        if (taxIdField) {
            taxIdField.addEventListener('blur', function() {
                const result = validateCanadianTaxId(this.value);
                if (result.isValid && this.value) {
                    this.value = formatBusinessNumber(this.value);
                }
                showValidationResult(this, result);
            });
        }
        
        // Registration Number validation with formatting
        const regNumberField = document.getElementById('Company_RegistrationNumber');
        if (regNumberField) {
            regNumberField.addEventListener('blur', function() {
                const result = validateCanadianBusinessNumber(this.value);
                if (result.isValid && this.value) {
                    this.value = formatBusinessNumber(this.value);
                }
                showValidationResult(this, result);
            });
        }
        
        // Established Date validation
        const establishedDateField = document.getElementById('Company_EstablishedDate');
        if (establishedDateField) {
            establishedDateField.addEventListener('blur', function() {
                const result = validateBusinessEstablishedDate(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Work Hours validation (if fields exist)
        const workHoursField = document.getElementById('Company_StandardWorkHours');
        if (workHoursField) {
            workHoursField.addEventListener('blur', function() {
                const result = validateCanadianWorkHours(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Overtime Rate validation (if fields exist)
        const overtimeRateField = document.getElementById('Company_OvertimeRate');
        if (overtimeRateField) {
            overtimeRateField.addEventListener('blur', function() {
                const result = validateCanadianOvertimeRate(this.value);
                showValidationResult(this, result);
            });
        }
    }
    
    function showValidationResult(field, result) {
        // Remove existing validation classes and messages
        field.classList.remove('is-valid', 'is-invalid');
        
        const existingFeedback = field.parentElement.querySelector('.invalid-feedback, .valid-feedback');
        if (existingFeedback) {
            existingFeedback.remove();
        }
        
        if (!result.isValid) {
            field.classList.add('is-invalid');
            
            const feedbackDiv = document.createElement('div');
            feedbackDiv.className = 'invalid-feedback';
            feedbackDiv.textContent = result.message;
            field.parentElement.appendChild(feedbackDiv);
        } else if (field.value && field.value.trim().length > 0) {
            field.classList.add('is-valid');
        }
    }
    
    // Form submission validation
    function validateCanadianBusinessForm() {
        let isValid = true;
        const errors = [];
        
        const form = document.querySelector('form');
        if (!form) return true;
        
        const fields = {
            Name: form.querySelector('#Company_Name'),
            Code: form.querySelector('#Company_Code'),
            Address: form.querySelector('#Company_Address'),
            City: form.querySelector('#Company_City'),
            State: form.querySelector('#Company_State'),
            ZipCode: form.querySelector('#Company_ZipCode'),
            Phone: form.querySelector('#Company_Phone'),
            Email: form.querySelector('#Company_Email'),
            Website: form.querySelector('#Company_Website'),
            TaxId: form.querySelector('#Company_TaxId'),
            RegistrationNumber: form.querySelector('#Company_RegistrationNumber'),
            EstablishedDate: form.querySelector('#Company_EstablishedDate'),
            StandardWorkHours: form.querySelector('#Company_StandardWorkHours'),
            OvertimeRate: form.querySelector('#Company_OvertimeRate')
        };
        
        // Validate required fields
        if (fields.Name) {
            const result = validateCanadianBusinessName(fields.Name.value);
            if (!result.isValid) {
                isValid = false;
                errors.push(`Company Name: ${result.message}`);
                showValidationResult(fields.Name, result);
            }
        }
        
        if (fields.Address) {
            const result = validateCanadianBusinessAddress(fields.Address.value);
            if (!result.isValid) {
                isValid = false;
                errors.push(`Business Address: ${result.message}`);
                showValidationResult(fields.Address, result);
            }
        }
        
        if (fields.EstablishedDate) {
            const result = validateBusinessEstablishedDate(fields.EstablishedDate.value);
            if (!result.isValid) {
                isValid = false;
                errors.push(`Established Date: ${result.message}`);
                showValidationResult(fields.EstablishedDate, result);
            }
        }
        
        // Validate optional fields if they have values
        Object.keys(fields).forEach(fieldName => {
            const field = fields[fieldName];
            if (!field || !field.value || field.value.trim().length === 0) return;
            
            let result = { isValid: true };
            
            switch(fieldName) {
                case 'Code':
                    result = validateBusinessCode(field.value);
                    break;
                case 'City':
                    result = validateCanadianBusinessCity(field.value);
                    break;
                case 'Phone':
                    if (window.CanadianEmployeeValidation) {
                        result = window.CanadianEmployeeValidation.validatePhoneNumber(field.value);
                    }
                    break;
                case 'State':
                    if (window.CanadianEmployeeValidation) {
                        result = window.CanadianEmployeeValidation.validateProvince(field.value);
                    }
                    break;
                case 'ZipCode':
                    if (window.CanadianEmployeeValidation) {
                        result = window.CanadianEmployeeValidation.validatePostalCode(field.value);
                    }
                    break;
                case 'Email':
                    result = validateCanadianBusinessEmail(field.value);
                    break;
                case 'Website':
                    result = validateCanadianBusinessWebsite(field.value);
                    break;
                case 'TaxId':
                    result = validateCanadianTaxId(field.value);
                    break;
                case 'RegistrationNumber':
                    result = validateCanadianBusinessNumber(field.value);
                    break;
                case 'StandardWorkHours':
                    result = validateCanadianWorkHours(field.value);
                    break;
                case 'OvertimeRate':
                    result = validateCanadianOvertimeRate(field.value);
                    break;
            }
            
            if (!result.isValid) {
                isValid = false;
                errors.push(`${fieldName}: ${result.message}`);
                showValidationResult(field, result);
            }
        });
        
        if (!isValid) {
            console.error('Canadian Business Validation Errors:', errors);
            
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'error',
                    title: 'Canadian Business Registration Validation Errors',
                    html: '<ul style="text-align: left;"><li>' + errors.join('</li><li>') + '</li></ul>',
                    confirmButtonText: 'Fix Errors'
                });
            }
        }
        
        return isValid;
    }
    
    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        setupBusinessFieldValidation();
        
        // Override form submission
        const form = document.querySelector('form');
        if (form) {
            form.addEventListener('submit', function(e) {
                if (!validateCanadianBusinessForm()) {
                    e.preventDefault();
                    return false;
                }
            });
        }
    });
    
    // Export validation functions for external use
    window.CanadianBusinessValidation = {
        validateBusinessName: validateCanadianBusinessName,
        validateBusinessCode: validateBusinessCode,
        validateBusinessAddress: validateCanadianBusinessAddress,
        validateBusinessCity: validateCanadianBusinessCity,
        validateBusinessEmail: validateCanadianBusinessEmail,
        validateBusinessWebsite: validateCanadianBusinessWebsite,
        validateBusinessNumber: validateCanadianBusinessNumber,
        validateTaxId: validateCanadianTaxId,
        validateEstablishedDate: validateBusinessEstablishedDate,
        validateWorkHours: validateCanadianWorkHours,
        validateOvertimeRate: validateCanadianOvertimeRate,
        formatBusinessNumber: formatBusinessNumber,
        formatBusinessCode: formatBusinessCode,
        validateForm: validateCanadianBusinessForm
    };
    
})();