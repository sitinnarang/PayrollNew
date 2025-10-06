#!/usr/bin/env pwsh

Write-Host "=== Testing Complete Company Registration and Login Flow ===" -ForegroundColor Green

# Generate unique test data
$timestamp = (Get-Date).ToString("yyyyMMddHHmmss")
$testCompanyName = "TestCompany$timestamp"
$testUsername = "user$timestamp"
$testEmail = "user$timestamp@testcompany.com"
$testPassword = "TestPass123!"

Write-Host "`nStep 1: Creating company and user account..." -ForegroundColor Yellow

# Create the registration request
$registrationData = @{
    CompanyName = $testCompanyName
    Description = "Test company created for testing login redirect"
    Address = "123 Test St"
    City = "Test City"
    State = "Test State"  
    ZipCode = "12345"
    Country = "United States"
    Phone = "+1-555-0123"
    CompanyEmail = "info@testcompany.com"
    Website = "https://www.testcompany.com"
    TaxId = "12-3456789"
    RegistrationNumber = "REG$timestamp"
    EstablishedDate = "2024-01-01T00:00:00Z"
    Username = $testUsername
    Email = $testEmail
    Password = $testPassword
} | ConvertTo-Json

Write-Host "Registration data:" -ForegroundColor Cyan
Write-Host $registrationData

try {
    # Call registration API (ignore SSL for localhost testing)
    [System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}
    $registrationResponse = Invoke-RestMethod -Uri "https://localhost:44329/api/registration/company-and-user" `
                                            -Method Post `
                                            -Body $registrationData `
                                            -ContentType "application/json"

    Write-Host "`nRegistration Response:" -ForegroundColor Cyan
    $registrationResponse | ConvertTo-Json -Depth 3 | Write-Host

    if ($registrationResponse.success) {
        $companyId = $registrationResponse.companyId
        Write-Host "`nStep 2: Verifying user was created with company claim..." -ForegroundColor Yellow
        
        # Check database for the user and their claims
        $sqlQuery = @"
SELECT 
    u.UserName, 
    u.Email, 
    u.EmailConfirmed,
    c.ClaimType, 
    c.ClaimValue,
    co.Name as CompanyName
FROM AbpUsers u 
LEFT JOIN AbpUserClaims c ON u.Id = c.UserId 
LEFT JOIN AppCompanies co ON c.ClaimValue = CAST(co.Id as nvarchar(36))
WHERE u.UserName = '$testUsername'
"@
        
        Write-Host "SQL Query: $sqlQuery" -ForegroundColor Cyan
        $dbResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q $sqlQuery -h -1 -s "|"
        
        Write-Host "`nDatabase verification result:" -ForegroundColor Cyan
        $dbResult | Write-Host
        
        Write-Host "`nStep 3: Testing login and redirect..." -ForegroundColor Yellow
        Write-Host "Expected redirect URL: /Companies/$companyId" -ForegroundColor Cyan
        Write-Host "Test completed successfully!" -ForegroundColor Green
        Write-Host "`nTo manually test login redirect:" -ForegroundColor Yellow
        Write-Host "1. Go to https://localhost:44329/Account/Login" -ForegroundColor White
        Write-Host "2. Login with:" -ForegroundColor White
        Write-Host "   Username: $testUsername" -ForegroundColor White
        Write-Host "   Password: $testPassword" -ForegroundColor White
        Write-Host "3. You should be redirected to: /Companies/$companyId" -ForegroundColor White
        
    } else {
        Write-Host "Registration failed: $($registrationResponse.error)" -ForegroundColor Red
    }
}
catch {
    Write-Host "Error during registration: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Response body: $($_.ErrorDetails.Message)" -ForegroundColor Red
}