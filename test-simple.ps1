# Test Company Creation and User Creation in AbpUsers Table
$baseUrl = "https://localhost:44329"

# Certificate bypass for localhost
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

Write-Host "=== Testing Company Registration and User Creation ===" -ForegroundColor Cyan

# Generate unique test data
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$testUser = "user_$timestamp"
$testEmail = "user_$timestamp@example.com"

Write-Host "Test Username: $testUser" -ForegroundColor Yellow
Write-Host "Test Email: $testEmail" -ForegroundColor Yellow

# Step 1: Get the registration page
Write-Host "`n1. Getting registration page..." -ForegroundColor Green
try {
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $response = Invoke-WebRequest -Uri "$baseUrl/Companies/CreateCompany" -WebSession $session

    # Extract anti-forgery token
    if ($response.Content -match '__RequestVerificationToken[^>]+value="([^"]+)"') {
        $token = $matches[1]
        Write-Host "Token extracted successfully" -ForegroundColor Green
    } else {
        Write-Host "Could not extract anti-forgery token" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "Failed to get registration page: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 2: Submit registration form
Write-Host "`n2. Submitting registration form..." -ForegroundColor Green
$formData = @{
    "__RequestVerificationToken" = $token
    "Company.Name" = "TestCompany_$timestamp"
    "Company.Code" = "TC_$timestamp"
    "Company.Description" = "Test company for user creation verification"
    "Company.Address" = "123 Test Street"
    "Company.City" = "TestCity"
    "Company.State" = "TX"
    "Company.ZipCode" = "12345"
    "Company.Country" = "United States"
    "Company.Phone" = "555-0123"
    "Company.Email" = "company_$timestamp@example.com"
    "Company.Website" = "https://test$timestamp.com"
    "Company.TaxId" = "12-$timestamp"
    "Company.RegistrationNumber" = "REG_$timestamp"
    "Company.EstablishedDate" = "2024-01-01"
    "Company.NumberOfEmployees" = "10"
    "Company.Industry" = "Technology"
    "Company.IsActive" = "true"
    "UserRegistration.Username" = $testUser
    "UserRegistration.Email" = $testEmail
    "UserRegistration.Password" = "1q2w3E*"
    "UserRegistration.ConfirmPassword" = "1q2w3E*"
}

try {
    $response = Invoke-WebRequest -Uri "$baseUrl/Companies/CreateCompany" -Method POST -Body $formData -WebSession $session
    $result = $response.Content | ConvertFrom-Json
    
    if ($result.success) {
        Write-Host "Registration successful!" -ForegroundColor Green
        Write-Host "Company ID: $($result.companyId)" -ForegroundColor White
        Write-Host "User ID: $($result.userId)" -ForegroundColor White
        Write-Host "Message: $($result.message)" -ForegroundColor White
    } else {
        Write-Host "Registration failed: $($result.error)" -ForegroundColor Red
        if ($result.fullError) {
            Write-Host "Full error: $($result.fullError)" -ForegroundColor Red
        }
        exit 1
    }
} catch {
    Write-Host "Request failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 3: Verify user exists in AbpUsers table
Write-Host "`n3. Verifying user in AbpUsers table..." -ForegroundColor Green
$userQuery = "SELECT Id, UserName, Email, EmailConfirmed, IsActive FROM AbpUsers WHERE UserName = '$testUser'"
$userResult = sqlcmd -S "localhost\SQLEXPRESS" -d "PayrollPro" -E -Q "$userQuery" -h -1 -W

if ($userResult -match $testUser) {
    Write-Host "SUCCESS: User '$testUser' found in AbpUsers table!" -ForegroundColor Green
} else {
    Write-Host "FAILURE: User '$testUser' NOT found in AbpUsers table!" -ForegroundColor Red
}

# Step 4: Show current users count
Write-Host "`n4. Current users in database:" -ForegroundColor Green
$countQuery = "SELECT COUNT(*) as UserCount FROM AbpUsers"
$count = sqlcmd -S "localhost\SQLEXPRESS" -d "PayrollPro" -E -Q "$countQuery" -h -1 -W
Write-Host "Total users: $count" -ForegroundColor White

$allUsersQuery = "SELECT UserName, Email, IsActive FROM AbpUsers"
$allUsers = sqlcmd -S "localhost\SQLEXPRESS" -d "PayrollPro" -E -Q "$allUsersQuery" -h -1 -W
Write-Host "All users:" -ForegroundColor White
Write-Host $allUsers -ForegroundColor Gray

Write-Host "`n=== Test Complete ===" -ForegroundColor Cyan
Write-Host "Login credentials:" -ForegroundColor Yellow
Write-Host "Username: $testUser" -ForegroundColor White  
Write-Host "Password: 1q2w3E*" -ForegroundColor White
Write-Host "Login URL: $baseUrl/Account/Login" -ForegroundColor White