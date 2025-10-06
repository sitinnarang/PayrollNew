# Simple test for registration API
$baseUrl = "https://localhost:44329"

# Certificate bypass
add-type @"
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    public class TrustAllCertsPolicy : ICertificatePolicy {
        public bool CheckValidationResult(
            ServicePoint srvPoint, X509Certificate certificate,
            WebRequest request, int certificateProblem) {
            return true;
        }
    }
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

Write-Host "=== Testing Registration API ===" -ForegroundColor Cyan

# Test data
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$testUser = "user_$timestamp"
$testEmail = "user_$timestamp@example.com"

Write-Host "Username: $testUser" -ForegroundColor Yellow
Write-Host "Email: $testEmail" -ForegroundColor Yellow

$requestBody = @{
    CompanyName = "TestCompany_$timestamp"
    Description = "Test company"
    Address = "123 Test St"
    City = "TestCity"
    State = "TX"
    ZipCode = "12345"
    Country = "United States"
    Phone = "555-0123"
    CompanyEmail = "company_$timestamp@example.com"
    Website = "https://test.com"
    TaxId = "12-345678"
    RegistrationNumber = "REG123"
    EstablishedDate = "2024-01-01T00:00:00"
    Username = $testUser
    Email = $testEmail
    Password = "1q2w3E*"
} | ConvertTo-Json

Write-Host "`nCalling API..." -ForegroundColor Green

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/registration/company-and-user" -Method POST -Body $requestBody -ContentType "application/json"
    
    Write-Host "SUCCESS!" -ForegroundColor Green
    Write-Host "Company ID: $($response.companyId)" -ForegroundColor White
    Write-Host "User ID: $($response.userId)" -ForegroundColor White
    Write-Host "Message: $($response.message)" -ForegroundColor White
    
    # Check database
    Write-Host "`nChecking database..." -ForegroundColor Green
    $query = "SELECT UserName FROM AbpUsers WHERE UserName = '$testUser'"
    $result = sqlcmd -S "localhost\SQLEXPRESS" -d "PayrollPro" -E -Q "$query" -h -1 -W
    
    if ($result -match $testUser) {
        Write-Host "✓ User found in database!" -ForegroundColor Green
    } else {
        Write-Host "✗ User NOT found" -ForegroundColor Red
    }
}
catch {
    Write-Host "FAILED: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
}

Write-Host "`nTest credentials:" -ForegroundColor Yellow
Write-Host "Username: $testUser" -ForegroundColor White
Write-Host "Password: 1q2w3E`*" -ForegroundColor White