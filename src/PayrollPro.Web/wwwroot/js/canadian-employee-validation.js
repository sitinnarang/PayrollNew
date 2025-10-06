// Canadian Employee Validation Script
// Provides comprehensive real-time validation for Canadian employment standards

window.CanadianEmployeeValidation = (function() {
    'use strict';

    // Validation patterns
    const patterns = {
        phoneNumber: /^(\d{3})(\d{3})(\d{4})$/,
        postalCode: /^[A-Z]\d[A-Z]\s?\d[A-Z]\d$/,
        sin: /^\d{3}\s?\d{3}\s?\d{3}$/,
        name: /^[a-zA-ZÀ-ÿ\s\-'\.]{2,50}$/,
        numericOnly: /^\d+(\.\d{1,2})?$/,
        alphanumeric: /^[a-zA-Z0-9\s\-_\.]+$/,
        employeeId: /^[a-zA-Z0-9\-_]{3,20}$/,
        email: /^[^\s@]+@[^\s@]+\.[^\s@]+$/
    };

    // Canadian provinces and territories
    const canadianProvinces = [
        'AB', 'BC', 'MB', 'NB', 'NL', 'NS', 'NT', 'NU', 'ON', 'PE', 'QC', 'SK', 'YT',
        'Alberta', 'British Columbia', 'Manitoba', 'New Brunswick', 'Newfoundland and Labrador',
        'Nova Scotia', 'Northwest Territories', 'Nunavut', 'Ontario', 'Prince Edward Island',
        'Quebec', 'Saskatchewan', 'Yukon'
    ];

    // Name validation - Canadian standards with French-Canadian support
    function validateCanadianName(name) {
        if (!name) {
            return { isValid: false, message: 'Name is required' };
        }

        if (name.length < 2) {
            return { isValid: false, message: 'Name must be at least 2 characters long' };
        }

        if (name.length > 50) {
            return { isValid: false, message: 'Name must not exceed 50 characters' };
        }

        if (!patterns.name.test(name)) {
            return { 
                isValid: false, 
                message: 'Name can only contain letters, spaces, hyphens, apostrophes, and accented characters' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Phone number validation - Canadian format (10 digits)
    function validateCanadianPhoneNumber(phone) {
        if (!phone) {
            return { isValid: false, message: 'Phone number is required' };
        }

        // Remove all non-numeric characters
        const cleanPhone = phone.replace(/\D/g, '');
        
        if (cleanPhone.length !== 10) {
            return { 
                isValid: false, 
                message: 'Phone number must be exactly 10 digits (e.g., 4165551234)' 
            };
        }

        // Check for valid Canadian area codes (simplified check)
        const areaCode = cleanPhone.substring(0, 3);
        const invalidAreaCodes = ['000', '001', '911'];
        
        if (invalidAreaCodes.includes(areaCode) || areaCode.startsWith('1')) {
            return { 
                isValid: false, 
                message: 'Invalid Canadian area code' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Format phone number
    function formatCanadianPhoneNumber(phone) {
        const cleanPhone = phone.replace(/\D/g, '');
        if (cleanPhone.length === 10) {
            return `(${cleanPhone.substring(0, 3)}) ${cleanPhone.substring(3, 6)}-${cleanPhone.substring(6)}`;
        }
        return phone;
    }

    // Postal code validation
    function validateCanadianPostalCode(postalCode) {
        if (!postalCode) {
            return { isValid: false, message: 'Postal code is required' };
        }

        const cleanPostal = postalCode.replace(/\s/g, '').toUpperCase();
        
        if (cleanPostal.length !== 6) {
            return { 
                isValid: false, 
                message: 'Postal code must be 6 characters (e.g., K1A 0A6)' 
            };
        }

        if (!patterns.postalCode.test(postalCode.toUpperCase())) {
            return { 
                isValid: false, 
                message: 'Invalid postal code format (e.g., K1A 0A6)' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Format postal code
    function formatCanadianPostalCode(postalCode) {
        const cleanPostal = postalCode.replace(/\s/g, '').toUpperCase();
        if (cleanPostal.length === 6) {
            return `${cleanPostal.substring(0, 3)} ${cleanPostal.substring(3)}`;
        }
        return postalCode;
    }

    // SIN validation using Luhn algorithm
    function validateCanadianSIN(sin) {
        if (!sin) {
            return { isValid: false, message: 'SIN is required' };
        }

        const cleanSIN = sin.replace(/\s/g, '');
        
        if (cleanSIN.length !== 9) {
            return { 
                isValid: false, 
                message: 'SIN must be 9 digits (e.g., 123 456 789)' 
            };
        }

        if (!/^\d{9}$/.test(cleanSIN)) {
            return { 
                isValid: false, 
                message: 'SIN can only contain digits' 
            };
        }

        // Luhn algorithm check
        let sum = 0;
        for (let i = 0; i < 9; i++) {
            let digit = parseInt(cleanSIN[i]);
            if (i % 2 === 1) {
                digit *= 2;
                if (digit > 9) {
                    digit = Math.floor(digit / 10) + (digit % 10);
                }
            }
            sum += digit;
        }

        if (sum % 10 !== 0) {
            return { 
                isValid: false, 
                message: 'Invalid SIN number' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Format SIN
    function formatCanadianSIN(sin) {
        const cleanSIN = sin.replace(/\D/g, '');
        if (cleanSIN.length === 9) {
            return `${cleanSIN.substring(0, 3)} ${cleanSIN.substring(3, 6)} ${cleanSIN.substring(6)}`;
        }
        return sin;
    }

    // Province validation
    function validateCanadianProvince(province) {
        if (!province) {
            return { isValid: false, message: 'Province is required' };
        }

        if (!canadianProvinces.includes(province)) {
            return { 
                isValid: false, 
                message: 'Must be a valid Canadian province or territory' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Employment age validation (14+ in most provinces)
    function validateEmploymentAge(dateOfBirth) {
        if (!dateOfBirth) {
            return { isValid: false, message: 'Date of birth is required' };
        }

        const birthDate = new Date(dateOfBirth);
        const today = new Date();
        
        if (isNaN(birthDate.getTime())) {
            return { 
                isValid: false, 
                message: 'Invalid date format' 
            };
        }

        const age = today.getFullYear() - birthDate.getFullYear();
        const monthDiff = today.getMonth() - birthDate.getMonth();
        
        const actualAge = monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate()) 
            ? age - 1 : age;

        if (actualAge < 14) {
            return { 
                isValid: false, 
                message: 'Employee must be at least 14 years old' 
            };
        }

        if (actualAge > 100) {
            return { 
                isValid: false, 
                message: 'Please verify the date of birth' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Salary validation - Canadian minimum wage compliance
    function validateCanadianSalary(salary) {
        if (!salary) {
            return { isValid: false, message: 'Salary is required' };
        }

        const numSalary = parseFloat(salary);
        
        if (isNaN(numSalary)) {
            return { 
                isValid: false, 
                message: 'Salary must be a valid number' 
            };
        }

        // Minimum wage varies by province, using federal minimum as base
        const federalMinimum = 17.30; // Federal minimum wage per hour
        const annualMinimum = federalMinimum * 40 * 52; // Full-time equivalent

        if (numSalary < annualMinimum) {
            return { 
                isValid: false, 
                message: `Salary should meet minimum wage standards (approximately $${annualMinimum.toFixed(0)} annually)` 
            };
        }

        if (numSalary > 1000000) {
            return { 
                isValid: false, 
                message: 'Please verify the salary amount' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Employee ID validation
    function validateEmployeeId(employeeId) {
        if (!employeeId) {
            return { isValid: false, message: 'Employee ID is required' };
        }

        if (employeeId.length < 3) {
            return { 
                isValid: false, 
                message: 'Employee ID must be at least 3 characters long' 
            };
        }

        if (employeeId.length > 20) {
            return { 
                isValid: false, 
                message: 'Employee ID must not exceed 20 characters' 
            };
        }

        if (!patterns.employeeId.test(employeeId)) {
            return { 
                isValid: false, 
                message: 'Employee ID can only contain letters, numbers, hyphens, and underscores' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Email validation
    function validateEmail(email) {
        if (!email) {
            return { isValid: false, message: 'Email is required' };
        }

        if (!patterns.email.test(email)) {
            return { 
                isValid: false, 
                message: 'Please enter a valid email address' 
            };
        }

        return { isValid: true, message: '' };
    }

    // City validation
    function validateCanadianCity(city) {
        if (!city) {
            return { isValid: false, message: 'City is required' };
        }

        if (city.length < 2) {
            return { 
                isValid: false, 
                message: 'City name must be at least 2 characters long' 
            };
        }

        if (city.length > 50) {
            return { 
                isValid: false, 
                message: 'City name must not exceed 50 characters' 
            };
        }

        return { isValid: true, message: '' };
    }

    // Date validation function - Updated for correct field IDs
    function validateDate(dateValue, fieldType) {
        if (!dateValue) {
            return { isValid: false, message: 'Date is required' };
        }

        const date = new Date(dateValue);
        const today = new Date();
        
        if (isNaN(date.getTime())) {
            return { 
                isValid: false, 
                message: 'Please enter a valid date' 
            };
        }

        switch (fieldType) {
            case 'Employee_DateOfBirth':
                return validateEmploymentAge(dateValue);
                
            case 'Employee_HireDate':
            case 'Employee_StartDate':
                // Hire date can be up to 1 year in the future
                const oneYearFromNow = new Date();
                oneYearFromNow.setFullYear(oneYearFromNow.getFullYear() + 1);
                
                if (date > oneYearFromNow) {
                    return { 
                        isValid: false, 
                        message: 'Hire date cannot be more than 1 year in the future' 
                    };
                }
                
                // Hire date shouldn't be too far in the past (50 years)
                const fiftyYearsAgo = new Date();
                fiftyYearsAgo.setFullYear(fiftyYearsAgo.getFullYear() - 50);
                
                if (date < fiftyYearsAgo) {
                    return { 
                        isValid: false, 
                        message: 'Please verify the hire date' 
                    };
                }
                break;
                
            case 'Employee_ReleaseDate':
                // Release date can be in the future
                const tenYearsFromNow = new Date();
                tenYearsFromNow.setFullYear(tenYearsFromNow.getFullYear() + 10);
                
                if (date > tenYearsFromNow) {
                    return { 
                        isValid: false, 
                        message: 'Release date seems too far in the future' 
                    };
                }
                break;
        }

        return { isValid: true, message: '' };
    }

    // Numeric field validation
    function validateNumericField(value, min = null, max = null) {
        if (!value) {
            return { isValid: false, message: 'This field is required' };
        }

        const numValue = parseFloat(value);
        
        if (isNaN(numValue)) {
            return { 
                isValid: false, 
                message: 'Please enter a valid number' 
            };
        }

        if (min !== null && numValue < min) {
            return { 
                isValid: false, 
                message: `Value must be at least ${min}` 
            };
        }

        if (max !== null && numValue > max) {
            return { 
                isValid: false, 
                message: `Value must not exceed ${max}` 
            };
        }

        return { isValid: true, message: '' };
    }

    // Show validation result
    function showValidationResult(field, result) {
        // Remove existing validation styling
        field.classList.remove('is-valid', 'is-invalid');
        
        // Find or create feedback element
        let feedbackElement = field.parentElement.querySelector('.invalid-feedback');
        if (!feedbackElement) {
            feedbackElement = document.createElement('div');
            feedbackElement.className = 'invalid-feedback';
            field.parentElement.appendChild(feedbackElement);
        }

        if (result.isValid) {
            field.classList.add('is-valid');
            feedbackElement.textContent = '';
            feedbackElement.style.display = 'none';
        } else {
            field.classList.add('is-invalid');
            feedbackElement.textContent = result.message;
            feedbackElement.style.display = 'block';
        }
    }

    // Input restriction functions
    function restrictToNumeric(input) {
        input.addEventListener('input', function(e) {
            // Allow only digits and one decimal point
            this.value = this.value.replace(/[^0-9.]/g, '');
            
            // Ensure only one decimal point
            const parts = this.value.split('.');
            if (parts.length > 2) {
                this.value = parts[0] + '.' + parts.slice(1).join('');
            }
            
            // Limit decimal places to 2
            if (parts[1] && parts[1].length > 2) {
                this.value = parts[0] + '.' + parts[1].substring(0, 2);
            }
        });
    }
    
    function restrictToPhoneNumber(input) {
        // Prevent non-numeric input at keypress level
        input.addEventListener('keypress', function(e) {
            // Allow only digits, backspace, delete, arrow keys, tab
            if (!/[0-9]/.test(e.key) && !['Backspace', 'Delete', 'ArrowLeft', 'ArrowRight', 'Tab'].includes(e.key)) {
                e.preventDefault();
            }
        });
        
        input.addEventListener('input', function(e) {
            // Remove all non-numeric characters immediately
            let cleaned = this.value.replace(/\D/g, '');
            
            // Strictly limit to 10 digits - truncate if longer
            if (cleaned.length > 10) {
                cleaned = cleaned.substring(0, 10);
            }
            
            // Set the cleaned value
            this.value = cleaned;
            
            // Real-time validation and formatting
            if (cleaned.length > 0) {
                const result = validateCanadianPhoneNumber(cleaned);
                showValidationResult(this, result);
                
                // Auto-format when exactly 10 digits
                if (result.isValid && cleaned.length === 10) {
                    this.value = formatCanadianPhoneNumber(cleaned);
                }
            } else {
                // Clear validation when empty
                input.classList.remove('is-valid', 'is-invalid');
                const feedbackElement = input.parentElement.querySelector('.invalid-feedback');
                if (feedbackElement) {
                    feedbackElement.style.display = 'none';
                }
            }
        });
        
        // Prevent paste of non-numeric content
        input.addEventListener('paste', function(e) {
            e.preventDefault();
            let pastedText = (e.clipboardData || window.clipboardData).getData('text');
            let cleaned = pastedText.replace(/\D/g, '');
            if (cleaned.length > 10) {
                cleaned = cleaned.substring(0, 10);
            }
            this.value = cleaned;
            // Trigger input event to process the pasted content
            this.dispatchEvent(new Event('input'));
        });
    }
    
    function restrictToAlphanumeric(input) {
        input.addEventListener('input', function(e) {
            // Allow letters, numbers, spaces, hyphens, underscores, dots
            this.value = this.value.replace(/[^a-zA-Z0-9\s\-_\.]/g, '');
        });
    }
    
    function restrictToEmployeeId(input) {
        input.addEventListener('input', function(e) {
            // Allow letters, numbers, hyphens, underscores
            this.value = this.value.replace(/[^a-zA-Z0-9\-_]/g, '');
            
            // Real-time validation
            if (this.value.length > 0) {
                const result = validateEmployeeId(this.value);
                showValidationResult(this, result);
            }
        });
    }

    // Real-time validation setup
    function setupFieldValidation() {
        // Name fields - letters only
        const nameFields = ['Employee_FirstName', 'Employee_LastName', 'Employee_DisplayName', 'Employee_Manager', 'Employee_EmergencyContactName'];
        nameFields.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                field.addEventListener('input', function(e) {
                    // Allow only letters, spaces, hyphens, apostrophes, and accented characters
                    this.value = this.value.replace(/[^a-zA-ZÀ-ÿ\s\-'\.]/g, '');
                    
                    // Real-time validation
                    if (this.value.length > 0) {
                        const result = validateCanadianName(this.value);
                        showValidationResult(this, result);
                    }
                });
                
                field.addEventListener('blur', function() {
                    const result = validateCanadianName(this.value);
                    showValidationResult(this, result);
                });
            }
        });
        
        // Employee ID - alphanumeric with restrictions
        const employeeIdField = document.getElementById('Employee_EmployeeId');
        if (employeeIdField) {
            restrictToEmployeeId(employeeIdField);
            employeeIdField.addEventListener('blur', function() {
                const result = validateEmployeeId(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Phone fields - numeric only, 10 digits max - FIXED IDs
        const phoneFields = ['Employee_Phone', 'Employee_MobilePhone', 'Employee_EmergencyContactPhone'];
        phoneFields.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                restrictToPhoneNumber(field);
                field.addEventListener('blur', function() {
                    const result = validateCanadianPhoneNumber(this.value);
                    showValidationResult(this, result);
                });
            }
        });
        
        // Salary field - numeric only - FIXED ID
        const salaryField = document.getElementById('Employee_Salary');
        if (salaryField) {
            restrictToNumeric(salaryField);
            salaryField.addEventListener('blur', function() {
                const result = validateCanadianSalary(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Date fields - FIXED IDs
        const dateFields = ['Employee_DateOfBirth', 'Employee_HireDate', 'Employee_StartDate', 'Employee_ReleaseDate'];
        dateFields.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                // Ensure floating label works for date inputs
                field.addEventListener('focus', function() {
                    this.classList.add('has-value');
                });
                
                field.addEventListener('blur', function() {
                    if (!this.value) {
                        this.classList.remove('has-value');
                    }
                    const result = validateDate(this.value, fieldId);
                    showValidationResult(this, result);
                });
                
                field.addEventListener('change', function() {
                    if (this.value) {
                        this.classList.add('has-value');
                    } else {
                        this.classList.remove('has-value');
                    }
                    const result = validateDate(this.value, fieldId);
                    showValidationResult(this, result);
                });
                
                // Check initial state
                if (field.value) {
                    field.classList.add('has-value');
                }
            }
        });
        
        // Email field - FIXED ID
        const emailField = document.getElementById('Employee_Email');
        if (emailField) {
            emailField.addEventListener('blur', function() {
                const result = validateEmail(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Address fields - alphanumeric - FIXED IDs
        const addressFields = ['Employee_Address', 'Employee_City'];
        addressFields.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                restrictToAlphanumeric(field);
                field.addEventListener('blur', function() {
                    if (fieldId === 'Employee_City') {
                        const result = validateCanadianCity(this.value);
                        showValidationResult(this, result);
                    }
                });
            }
        });
        
        // Postal Code - FIXED ID
        const postalCodeField = document.getElementById('Employee_PostalCode');
        if (postalCodeField) {
            postalCodeField.addEventListener('input', function() {
                // Format postal code as user types
                let cleaned = this.value.replace(/[^a-zA-Z0-9]/g, '').toUpperCase();
                if (cleaned.length > 6) cleaned = cleaned.substring(0, 6);
                
                if (cleaned.length > 3) {
                    this.value = cleaned.substring(0, 3) + ' ' + cleaned.substring(3);
                } else {
                    this.value = cleaned;
                }
                
                // Real-time validation
                if (cleaned.length === 6) {
                    const result = validateCanadianPostalCode(this.value);
                    showValidationResult(this, result);
                }
            });
            
            postalCodeField.addEventListener('blur', function() {
                const result = validateCanadianPostalCode(this.value);
                showValidationResult(this, result);
            });
        }
        
        // Province field - FIXED ID
        const provinceField = document.getElementById('Employee_Province');
        if (provinceField) {
            provinceField.addEventListener('change', function() {
                const result = validateCanadianProvince(this.value);
                showValidationResult(this, result);
            });
        }
        
        // SIN field - FIXED ID and Enhanced Validation
        const sinField = document.getElementById('Employee_SocialSecurityNumber');
        if (sinField) {
            sinField.addEventListener('input', function(e) {
                // Remove all non-numeric characters immediately
                let cleaned = this.value.replace(/\D/g, '');
                
                // Limit to 9 digits maximum
                if (cleaned.length > 9) {
                    cleaned = cleaned.substring(0, 9);
                }
                
                // Apply formatting with spaces
                if (cleaned.length > 6) {
                    this.value = cleaned.substring(0, 3) + ' ' + 
                               cleaned.substring(3, 6) + ' ' + 
                               cleaned.substring(6);
                } else if (cleaned.length > 3) {
                    this.value = cleaned.substring(0, 3) + ' ' + 
                               cleaned.substring(3);
                } else {
                    this.value = cleaned;
                }
                
                // Real-time validation feedback
                if (cleaned.length > 0) {
                    const result = validateCanadianSIN(this.value);
                    showValidationResult(this, result);
                }
            });
            
            sinField.addEventListener('blur', function() {
                const result = validateCanadianSIN(this.value);
                showValidationResult(this, result);
            });
        }
    }

    // Complete form validation
    function validateCanadianEmployeeForm() {
        let isValid = true;
        const errors = [];

        // Validate all required fields - UPDATED with correct IDs
        const requiredFields = [
            { id: 'Employee_FirstName', validator: validateCanadianName, label: 'First Name' },
            { id: 'Employee_LastName', validator: validateCanadianName, label: 'Last Name' },
            { id: 'Employee_Email', validator: validateEmail, label: 'Email' },
            { id: 'Employee_Phone', validator: validateCanadianPhoneNumber, label: 'Phone' },
            { id: 'Employee_SocialSecurityNumber', validator: validateCanadianSIN, label: 'SIN' },
            { id: 'Employee_DateOfBirth', validator: validateEmploymentAge, label: 'Date of Birth' },
            { id: 'Employee_Address', validator: (v) => v ? { isValid: true } : { isValid: false, message: 'Address is required' }, label: 'Address' },
            { id: 'Employee_City', validator: validateCanadianCity, label: 'City' },
            { id: 'Employee_PostalCode', validator: validateCanadianPostalCode, label: 'Postal Code' },
            { id: 'Employee_Province', validator: validateCanadianProvince, label: 'Province' }
        ];

        requiredFields.forEach(fieldInfo => {
            const field = document.getElementById(fieldInfo.id);
            if (field && field.value) {
                const result = fieldInfo.validator(field.value);
                showValidationResult(field, result);
                
                if (!result.isValid) {
                    isValid = false;
                    errors.push(`${fieldInfo.label}: ${result.message}`);
                }
            } else if (field) {
                const result = { isValid: false, message: `${fieldInfo.label} is required` };
                showValidationResult(field, result);
                isValid = false;
                errors.push(result.message);
            }
        });

        return { isValid, errors };
    }

    // Initialize when DOM is ready
    function initialize() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', setupFieldValidation);
        } else {
            setupFieldValidation();
        }
    }

    // Auto-initialize
    initialize();

    // Public API
    return {
        validateName: validateCanadianName,
        validatePhone: validateCanadianPhoneNumber,
        validatePostalCode: validateCanadianPostalCode,
        validateSIN: validateCanadianSIN,
        validateProvince: validateCanadianProvince,
        validateEmploymentAge: validateEmploymentAge,
        validateSalary: validateCanadianSalary,
        validateEmployeeId: validateEmployeeId,
        validateEmail: validateEmail,
        validateCity: validateCanadianCity,
        validateDate: validateDate,
        validateNumericField: validateNumericField,
        formatPostalCode: formatCanadianPostalCode,
        formatPhoneNumber: formatCanadianPhoneNumber,
        formatSIN: formatCanadianSIN,
        validateForm: validateCanadianEmployeeForm,
        setupFieldValidation: setupFieldValidation
    };

})();