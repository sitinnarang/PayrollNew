# Test the new registration API endpoint
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

Write-Host "=== Testing Registration API Endpoint ===" -ForegroundColor Cyan

# Generate unique test data
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$testUser = "user_$timestamp"
$testEmail = "user_$timestamp@example.com"

Write-Host "Test Username: $testUser" -ForegroundColor Yellow
Write-Host "Test Email: $testEmail" -ForegroundColor Yellow

# Create request body
$requestBody = @{
    CompanyName = "TestCompany_$timestamp"
    Description = "Test company for user creation verification"
    Address = "123 Test Street"
    City = "TestCity"
    State = "TX"
    ZipCode = "12345"
    Country = "United States"
    Phone = "555-0123"
    CompanyEmail = "company_$timestamp@example.com"
    Website = "https://test$timestamp.com"
    TaxId = "12-$timestamp"
    RegistrationNumber = "REG_$timestamp"
    EstablishedDate = "2024-01-01T00:00:00"
    Username = $testUser
    Email = $testEmail
    Password = "1q2w3E*"
} | ConvertTo-Json

Write-Host "`nSending registration request to API..." -ForegroundColor Green
Write-Host "URL: $baseUrl/api/registration/company-and-user" -ForegroundColor White

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/registration/company-and-user" `
                                 -Method POST `
                                 -Body $requestBody `
                                 -ContentType "application/json"
    
    if ($response.success) {
        Write-Host "✓ SUCCESS: Company and user created via API!" -ForegroundColor Green
        Write-Host "  Company ID: $($response.companyId)" -ForegroundColor White
        Write-Host "  User ID: $($response.userId)" -ForegroundColor White
        Write-Host "  Username: $($response.username)" -ForegroundColor White
        Write-Host "  Email: $($response.email)" -ForegroundColor White
        Write-Host "  Message: $($response.message)" -ForegroundColor White
        
        # Verify user exists in database
        Write-Host "`nVerifying user in database..." -ForegroundColor Green
        $userQuery = "SELECT Id, UserName, Email, EmailConfirmed, IsActive FROM AbpUsers WHERE UserName = '$testUser'"
        $userResult = sqlcmd -S "localhost\SQLEXPRESS" -d "PayrollPro" -E -Q "$userQuery" -h -1 -W
        
        if ($userResult -match $testUser) {
            Write-Host "✓ SUCCESS: User '$testUser' found in AbpUsers table!" -ForegroundColor Green
        } else {
            Write-Host "✗ WARNING: User not found in database" -ForegroundColor Yellow
        }
        
        # Show current user count
        $countQuery = "SELECT COUNT(*) as UserCount FROM AbpUsers"
        $count = sqlcmd -S "localhost\SQLEXPRESS" -d "PayrollPro" -E -Q "$countQuery" -h -1 -W
        Write-Host "Total users in database: $count" -ForegroundColor White
        
    } else {
        Write-Host "✗ FAILED: $($response.error)" -ForegroundColor Red
    }
} catch {
    Write-Host "✗ API call failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $statusCode = $_.Exception.Response.StatusCode.value__
        Write-Host "Status Code: $statusCode" -ForegroundColor Red
    }
}

Write-Host "`n=== API Test Complete ===" -ForegroundColor Cyan
Write-Host "Test credentials for manual login verification:" -ForegroundColor Yellow
Write-Host "Username: $testUser" -ForegroundColor White
Write-Host "Password: 1q2w3E*" -ForegroundColor White
Write-Host "Login URL: $baseUrl/Account/Login" -ForegroundColor White