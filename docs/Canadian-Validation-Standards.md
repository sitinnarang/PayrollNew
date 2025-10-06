# Canadian Employee Validation Standards

This document outlines the comprehensive Canadian validation standards implemented in the PayrollPro employee creation system.

## Overview

The system enforces Canadian employment and regulatory standards to ensure compliance with federal and provincial employment laws, tax regulations, and data protection requirements.

## Validation Categories

### 1. Personal Information Validation

#### Name Validation (`CanadianNameAttribute`)
- **Purpose**: Validates employee names including French-Canadian names with accents and special characters
- **Rules**:
  - Minimum 2 characters, maximum 50 characters
  - Allows letters (including accented characters À-ÿ), spaces, hyphens, and apostrophes
  - Supports French-Canadian naming conventions (e.g., Jean-Baptiste, O'Connor, Marie-Claire)
- **Applied to**: First Name, Last Name, Emergency Contact Name, Manager Name, Preferred Name

#### Employee ID Validation (`CanadianEmployeeIdAttribute`)
- **Purpose**: Ensures employee IDs follow Canadian organizational standards
- **Rules**:
  - Minimum 3 characters, maximum 20 characters
  - Alphanumeric characters, hyphens, and underscores only
  - No spaces or special characters (except - and _)
- **Applied to**: Employee ID field

### 2. Contact Information Validation

#### Phone Number Validation (`CanadianPhoneNumberAttribute`)
- **Purpose**: Validates Canadian phone numbers according to NANP (North American Numbering Plan)
- **Rules**:
  - Must be exactly 10 digits (excluding country code)
  - Accepts country code +1 (11 digits total)
  - Area code cannot start with 0 or 1
  - Phone number cannot start with 0 or 1
  - Supports various formats: (416) 555-1234, 416-555-1234, 416.555.1234, 4165551234
- **Applied to**: Phone, Mobile Phone, Emergency Contact Phone
- **Auto-formatting**: Automatically formats to (416) 555-1234 format

#### Address Validation

##### Postal Code Validation (`CanadianPostalCodeAttribute`)
- **Purpose**: Validates Canadian postal codes according to Canada Post standards
- **Rules**:
  - Format: Letter-Digit-Letter Digit-Letter-Digit (e.g., K1A 0A6)
  - First letter cannot be D, F, I, O, Q, U, W, Z
  - Third letter cannot be D, F, I, O, Q, U
  - Case-insensitive input, automatically converted to uppercase
- **Applied to**: Postal Code field
- **Auto-formatting**: Automatically adds space if missing (K1A0A6 → K1A 0A6)

##### Province/Territory Validation (`CanadianProvinceAttribute`)
- **Purpose**: Validates Canadian province and territory codes
- **Rules**:
  - Must be valid 2-letter province/territory code
  - Valid codes: AB, BC, MB, NB, NL, NS, NT, NU, ON, PE, QC, SK, YT
- **Applied to**: Province/Territory field

### 3. Government Identification

#### Social Insurance Number (SIN) Validation (`CanadianSINAttribute`)
- **Purpose**: Validates Canadian Social Insurance Numbers according to CRA standards
- **Rules**:
  - Must be exactly 9 digits
  - Cannot start with 0 or 8 (reserved numbers)
  - Must pass Luhn algorithm checksum validation
  - Accepts formats: 123-456-789, 123 456 789, 123456789
- **Applied to**: Social Insurance Number field
- **Auto-formatting**: Automatically formats to 123-456-789 format
- **Privacy**: Optional field, properly secured when stored

### 4. Employment Standards Compliance

#### Age Validation (`CanadianEmploymentAgeAttribute`)
- **Purpose**: Ensures compliance with Canadian employment age requirements
- **Rules**:
  - Minimum age: 14 years (federal standard, may vary by province)
  - Maximum age: 100 years (for data validation)
  - Age calculated based on current date
- **Applied to**: Date of Birth field

#### Salary Validation (`CanadianSalaryAttribute`)
- **Purpose**: Ensures salary meets Canadian minimum wage requirements
- **Rules**:
  - Minimum: Based on federal minimum wage ($17.30/hour as of 2024)
  - Calculated as annual salary ÷ (40 hours/week × 52 weeks)
  - Maximum: $500,000 CAD (reasonable upper limit)
  - Currency: Canadian Dollars (CAD)
- **Applied to**: Annual Salary field

### 5. Client-Side Validation Features

#### Real-Time Validation
- **Field-by-field validation**: Immediate feedback as user completes each field
- **Visual feedback**: Green checkmarks for valid fields, red borders for errors
- **Auto-formatting**: Automatic formatting for phone numbers, postal codes, and SIN
- **Error messages**: Clear, actionable error messages in English

#### Form Submission Validation
- **Comprehensive validation**: All fields validated before submission
- **Error summary**: List of all validation errors if submission fails
- **Loading indicators**: Visual feedback during form processing
- **Canadian compliance check**: Final validation against all Canadian standards

## Technical Implementation

### Backend Validation Attributes
- **Custom ValidationAttribute classes**: Server-side validation using .NET attributes
- **Localization ready**: Error messages can be localized for French-Canadian users
- **Extensible**: Easy to add new provinces or update regulations

### Frontend Validation Script
- **JavaScript validation**: `canadian-employee-validation.js`
- **Progressive enhancement**: Works with or without JavaScript
- **Mobile-friendly**: Touch-optimized for mobile devices
- **Accessibility**: Screen reader compatible error messages

### Integration Points
- **ASP.NET Core Model Validation**: Seamless integration with existing validation pipeline
- **Entity Framework**: Validation occurs before database operations
- **API endpoints**: Validation applies to both web forms and API requests

## Compliance Standards

### Federal Requirements
- **Employment Standards Act**: Age and wage requirements
- **Personal Information Protection Act**: Data handling for SIN and personal information
- **Canada Revenue Agency**: SIN validation and tax compliance

### Provincial Considerations
- **Minimum wage variations**: System can be configured for provincial differences
- **Employment age variations**: Some provinces have different minimum ages
- **Workers' compensation**: Framework ready for provincial WCB numbers

## Security and Privacy

### Data Protection
- **SIN encryption**: Social Insurance Numbers encrypted at rest
- **Audit logging**: All validation failures logged for compliance
- **Data minimization**: Only required fields marked as mandatory

### Validation Bypass Protection
- **Server-side enforcement**: All validation enforced on server regardless of client-side status
- **Input sanitization**: All user input sanitized before validation
- **SQL injection protection**: Parameterized queries and Entity Framework protection

## Configuration

### Customizable Settings
- **Minimum wage rates**: Configurable by province/territory
- **Employment ages**: Configurable minimum ages by jurisdiction
- **Validation messages**: Customizable error messages for branding

### Feature Flags
- **SIN validation**: Can be disabled for organizations not requiring SIN
- **Postal code validation**: Can be relaxed for international employees
- **Phone validation**: Can accommodate international employees with foreign numbers

## Testing and Quality Assurance

### Validation Test Cases
- **Positive tests**: Valid Canadian data formats
- **Negative tests**: Invalid formats and edge cases
- **Cross-browser testing**: Validated across major browsers
- **Mobile testing**: Touch and keyboard input validation

### Example Valid Inputs
- **Name**: "Jean-Baptiste O'Connor", "Marie-Ève Tremblay"
- **Phone**: "(416) 555-1234", "604.555.9876"
- **Postal Code**: "K1A 0A6", "V5K 0A1"
- **SIN**: "123-456-782" (passes Luhn check)
- **Province**: "ON", "BC", "QC"

## Future Enhancements

### Planned Improvements
- **Bilingual support**: French-Canadian error messages
- **Provincial wage integration**: Real-time minimum wage updates
- **Indigenous community support**: Special considerations for First Nations employment
- **Temporary worker validation**: Support for temporary foreign worker documentation

### Integration Roadmap
- **CRA integration**: Direct SIN validation with Canada Revenue Agency
- **Provincial systems**: Integration with provincial employment databases
- **Benefits validation**: CPP, EI, and other benefit program validation

## Support and Documentation

For technical support or questions about Canadian employment validation standards, please refer to:
- Government of Canada Employment Standards: https://www.canada.ca/en/employment-social-development/
- Canada Revenue Agency: https://www.canada.ca/en/revenue-agency/
- Provincial employment standards offices

---

**Note**: This validation system is designed to meet current Canadian standards as of 2024. Employment standards and regulations may change, and the system should be regularly updated to maintain compliance.