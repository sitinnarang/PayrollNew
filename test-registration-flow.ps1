# Test Registration and Login Flow
# This script tests the complete company registration and user login flow

# Certificate bypass for local HTTPS
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

$baseUrl = "https://localhost:44329"

Write-Host "=== PayrollPro Registration and Login Flow Test ===" -ForegroundColor Cyan

# Test data
$testCompany = @{
    name = "Test Company $(Get-Random)"
    address = "123 Test Street"
    city = "Test City"
    state = "Test State"
    zipCode = "12345"
    phone = "555-123-4567"
    email = "test@company.com"
    website = "https://testcompany.com"
    establishedDate = "2024-01-01"
    numberOfEmployees = "50"
    industry = "Technology"
    isActive = "true"
}

$testUser = @{
    username = "testuser$(Get-Random)"
    email = "testuser$(Get-Random)@test.com"
    password = "1q2w3E*"
    confirmPassword = "1q2w3E*"
}

Write-Host "Test Company: $($testCompany.name)" -ForegroundColor Yellow
Write-Host "Test User: $($testUser.username)" -ForegroundColor Yellow

# Step 1: Get the registration page and extract anti-forgery token
Write-Host "`n1. Getting registration page..." -ForegroundColor Green
try {
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $response = Invoke-WebRequest -Uri "$baseUrl/Companies/CreateCompany" -WebSession $session
    
    # Extract anti-forgery token
    $tokenMatch = $response.Content | Select-String '__RequestVerificationToken.*?value="([^"]+)"'
    if ($tokenMatch.Matches.Count -eq 0) {
        Write-Host "Could not find anti-forgery token" -ForegroundColor Red
        exit 1
    }
    $token = $tokenMatch.Matches[0].Groups[1].Value
    Write-Host "✓ Anti-forgery token extracted" -ForegroundColor Green
}
catch {
    Write-Host "✗ Failed to get registration page: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 2: Submit registration form
Write-Host "`n2. Submitting registration form..." -ForegroundColor Green
try {
    $formData = @{
        "__RequestVerificationToken" = $token
        "Company.Name" = $testCompany.name
        "Company.Address" = $testCompany.address
        "Company.City" = $testCompany.city
        "Company.State" = $testCompany.state
        "Company.ZipCode" = $testCompany.zipCode
        "Company.Phone" = $testCompany.phone
        "Company.Email" = $testCompany.email
        "Company.Website" = $testCompany.website
        "Company.EstablishedDate" = $testCompany.establishedDate
        "Company.NumberOfEmployees" = $testCompany.numberOfEmployees
        "Company.Industry" = $testCompany.industry
        "Company.IsActive" = $testCompany.isActive
        "UserRegistration.Username" = $testUser.username
        "UserRegistration.Email" = $testUser.email
        "UserRegistration.Password" = $testUser.password
        "UserRegistration.ConfirmPassword" = $testUser.confirmPassword
    }
    
    $response = Invoke-WebRequest -Uri "$baseUrl/Companies/CreateCompany" -Method POST -Body $formData -WebSession $session
    $result = $response.Content | ConvertFrom-Json
    
    if ($result.success) {
        Write-Host "✓ Registration successful!" -ForegroundColor Green
        Write-Host "  Company ID: $($result.companyId)" -ForegroundColor White
        Write-Host "  User ID: $($result.userId)" -ForegroundColor White
        Write-Host "  Message: $($result.message)" -ForegroundColor White
    }
    else {
        Write-Host "✗ Registration failed: $($result.error)" -ForegroundColor Red
        if ($result.fullError) {
            Write-Host "Full error: $($result.fullError)" -ForegroundColor Red
        }
        exit 1
    }
}
catch {
    Write-Host "✗ Registration request failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $errorContent = $_.Exception.Response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($errorContent)
        $errorText = $reader.ReadToEnd()
        Write-Host "Response content: $errorText" -ForegroundColor Red
    }
    exit 1
}

# Step 3: Verify user was created in database
Write-Host "`n3. Verifying user creation in database..." -ForegroundColor Green
try {
    $query = "SELECT Id, UserName, Email, EmailConfirmed, IsActive FROM AbpUsers WHERE UserName = '$($testUser.username)'"
    $dbResult = sqlcmd -S "localhost\SQLEXPRESS" -d "PayrollPro" -E -Q "$query" -h -1
    
    if ($dbResult -match $testUser.username) {
        Write-Host "✓ User found in database" -ForegroundColor Green
        Write-Host "Database record: $dbResult" -ForegroundColor White
    }
    else {
        Write-Host "✗ User not found in database" -ForegroundColor Red
        Write-Host "Query result: $dbResult" -ForegroundColor Red
    }
}
catch {
    Write-Host "✗ Database verification failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 4: Test login with new credentials
Write-Host "`n4. Testing login with new credentials..." -ForegroundColor Green
try {
    # Get login page and token
    $loginSession = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $loginPageResponse = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -WebSession $loginSession
    
    # Extract anti-forgery token for login
    $loginTokenMatch = $loginPageResponse.Content | Select-String '__RequestVerificationToken.*?value="([^"]+)"'
    if ($loginTokenMatch.Matches.Count -eq 0) {
        Write-Host "Could not find login anti-forgery token" -ForegroundColor Red
        return
    }
    $loginToken = $loginTokenMatch.Matches[0].Groups[1].Value
    
    # Submit login form
    $loginData = @{
        "__RequestVerificationToken" = $loginToken
        "UserNameOrEmailAddress" = $testUser.username
        "Password" = $testUser.password
        "RememberMe" = "false"
    }
    
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method POST -Body $loginData -WebSession $loginSession
    
    # Check if login was successful (usually redirects to dashboard or shows success)
    if ($loginResponse.StatusCode -eq 200 -and $loginResponse.BaseResponse.ResponseUri.PathAndQuery -ne "/Account/Login") {
        Write-Host "✓ Login successful! Redirected to: $($loginResponse.BaseResponse.ResponseUri)" -ForegroundColor Green
    }
    elseif ($loginResponse.Content -notmatch "invalid|error|failed") {
        Write-Host "✓ Login appears successful (no error messages found)" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Login may have failed" -ForegroundColor Yellow
        Write-Host "Response URL: $($loginResponse.BaseResponse.ResponseUri)" -ForegroundColor White
    }
}
catch {
    Write-Host "✗ Login test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== Test Summary ===" -ForegroundColor Cyan
Write-Host "Test User: $($testUser.username)" -ForegroundColor White
Write-Host "Test Email: $($testUser.email)" -ForegroundColor White
Write-Host "Test Password: $($testUser.password)" -ForegroundColor White
Write-Host "`nYou can now manually test login at: $baseUrl/Account/Login" -ForegroundColor Yellow