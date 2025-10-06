# Simple test for registration and login
$baseUrl = "https://localhost:44329"

# Generate unique identifiers
$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$username = "testuser_$timestamp"
$email = "$username@test.com"
$password = "1q2w3E*"

Write-Host "Testing registration and login flow..." -ForegroundColor Green
Write-Host "Username: $username" -ForegroundColor Yellow
Write-Host "Email: $email" -ForegroundColor Yellow
Write-Host "Password: $password" -ForegroundColor Yellow

# Step 1: Create user through registration API
Write-Host "`n1. Creating user through registration API..." -ForegroundColor Cyan

$registrationData = @{
    companyName = "Test Company $timestamp"
    description = "Test company created via API"
    address = "123 Test Street"
    city = "Test City"
    state = "TX"
    zipCode = "12345"
    country = "United States"
    phone = "555-0123"
    companyEmail = "company_$timestamp@test.com"
    website = "https://test$timestamp.com"
    taxId = "12-345678"
    registrationNumber = "REG$timestamp"
    establishedDate = "2024-01-01"
    username = $username
    email = $email
    password = $password
}

try {
    $registrationResponse = Invoke-RestMethod -Uri "$baseUrl/api/registration/company-and-user" `
        -Method POST `
        -Body ($registrationData | ConvertTo-Json -Depth 10) `
        -ContentType "application/json" `
        -SkipCertificateCheck

    Write-Host "Registration Response:" -ForegroundColor Green
    $registrationResponse | ConvertTo-Json -Depth 10 | Write-Host

    if ($registrationResponse.success) {
        Write-Host "✓ User created successfully!" -ForegroundColor Green
        Write-Host "User ID: $($registrationResponse.userId)" -ForegroundColor Yellow
        Write-Host "Company ID: $($registrationResponse.companyId)" -ForegroundColor Yellow
    } else {
        Write-Host "✗ Registration failed: $($registrationResponse.error)" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "✗ Registration API call failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Step 2: Verify user exists in database
Write-Host "`n2. Verifying user exists in database..." -ForegroundColor Cyan
try {
    $sqlQuery = "SELECT Id, UserName, Email, EmailConfirmed, IsActive, LEN(PasswordHash) AS PasswordHashLength FROM AbpUsers WHERE Email = '$email'"
    $dbResult = sqlcmd -S '.\SQLEXPRESS' -d 'PayrollPro' -E -Q "$sqlQuery"

    if ($dbResult -match $email) {
        Write-Host "✓ User found in database!" -ForegroundColor Green
    } else {
        Write-Host "✗ User not found in database!" -ForegroundColor Red
    }
} catch {
    Write-Host "Database check failed: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host "`nNow test login manually at: $baseUrl/Account/Login" -ForegroundColor Cyan
Write-Host "Use credentials:" -ForegroundColor Cyan
Write-Host "Email: $email" -ForegroundColor Yellow
Write-Host "Password: $password" -ForegroundColor Yellow

Write-Host "`nTest completed!" -ForegroundColor Green