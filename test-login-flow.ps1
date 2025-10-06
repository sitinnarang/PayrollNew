# Test the complete registration and login flow
$baseUrl = "https://localhost:44329"

# Generate unique identifiers
$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$username = "testuser_$timestamp"
$email = "$username@test.com"
$password = "1q2w3E*"

Write-Host "Testing complete registration and login flow..." -ForegroundColor Green
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

# Step 2: Wait a moment for any async operations
Write-Host "`n2. Waiting for user to be fully created..." -ForegroundColor Cyan
Start-Sleep -Seconds 2

# Step 3: Verify user exists in database
Write-Host "`n3. Verifying user exists in database..." -ForegroundColor Cyan
try {
    $sqlQuery = "SELECT Id, UserName, Email, EmailConfirmed, IsActive, LEN(PasswordHash) AS PasswordHashLength FROM AbpUsers WHERE Email = '$email'"
    $dbResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q "$sqlQuery" -h -1

    if ($dbResult -match $email) {
        Write-Host "✓ User found in database!" -ForegroundColor Green
        $dbResult | Write-Host
    } else {
        Write-Host "✗ User not found in database!" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "✗ Database verification failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Step 4: Test login via web form simulation
Write-Host "`n4. Testing login via web form..." -ForegroundColor Cyan

# First get the login page to get any required tokens
try {
    $loginPageResponse = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -SessionVariable session -SkipCertificateCheck
    Write-Host "✓ Login page accessed successfully" -ForegroundColor Green

    # Extract antiforgery token if present
    $antiforgeryToken = ""
    if ($loginPageResponse.Content -match 'name="__RequestVerificationToken"[^>]*value="([^"]*)"') {
        $antiforgeryToken = $matches[1]
        Write-Host "✓ Antiforgery token extracted: $($antiforgeryToken.Substring(0, 20))..." -ForegroundColor Green
    }

    # Attempt login
    Write-Host "Attempting login with credentials..." -ForegroundColor Yellow
    
    $loginData = @{
        UserNameOrEmailAddress = $email
        Password = $password
        RememberMe = "false"
    }
    
    if ($antiforgeryToken) {
        $loginData["__RequestVerificationToken"] = $antiforgeryToken
    }

    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/Account/Login" `
        -Method POST `
        -Body $loginData `
        -WebSession $session `
        -SkipCertificateCheck `
        -AllowUnencryptedAuthentication

    # Check if login was successful (redirect or successful page)
    if ($loginResponse.StatusCode -eq 200) {
        if ($loginResponse.Content -match "Invalid") {
            Write-Host "✗ Login failed - Invalid credentials" -ForegroundColor Red
        } else {
            Write-Host "✓ Login appears successful!" -ForegroundColor Green
        }
    } elseif ($loginResponse.StatusCode -eq 302) {
        Write-Host "✓ Login successful - redirected!" -ForegroundColor Green
    }
    
    Write-Host "Response Status: $($loginResponse.StatusCode)" -ForegroundColor Yellow
    
} catch {
    Write-Host "✗ Login test failed: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Response Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Yellow
    }
}

Write-Host "`nTest completed!" -ForegroundColor Green