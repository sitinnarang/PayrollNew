# Simple Employee Creation Test
Write-Host "Testing Employee Creation Form Submission..." -ForegroundColor Green

# Disable SSL validation
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Get the create page
    Write-Host "1. Loading employee creation page..." -ForegroundColor Cyan
    $createPage = Invoke-WebRequest -Uri "https://localhost:44329/Employees/Create" -Method GET -SessionVariable session
    Write-Host "   Status: $($createPage.StatusCode)" -ForegroundColor Green
    
    # Check if form exists
    if ($createPage.Content.Contains("employeeForm")) {
        Write-Host "   ✓ Employee form found" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Employee form not found" -ForegroundColor Red
        return
    }
    
    # Extract anti-forgery token (simple approach)
    $tokenMatch = $createPage.Content | Select-String '__RequestVerificationToken.*?value="([^"]+)"'
    $token = ""
    if ($tokenMatch) {
        $token = ([regex]'value="([^"]+)"').Match($createPage.Content).Groups[1].Value
        Write-Host "   ✓ Token extracted: $($token.Substring(0,10))..." -ForegroundColor Green
    }
    
    # Prepare test data
    Write-Host "2. Preparing Canadian test employee data..." -ForegroundColor Cyan
    $testEmployee = @{
        '__RequestVerificationToken' = $token
        'Employee.FirstName' = 'Marie'
        'Employee.LastName' = 'Dupont'
        'Employee.Email' = 'marie.dupont@test.ca'
        'Employee.Phone' = '514-555-9999'
        'Employee.EmployeeId' = 'EMP-TEST-001'
        'Employee.Department' = 'Marketing'
        'Employee.Position' = 'Marketing Manager'
        'Employee.HireDate' = '2024-01-15'
        'Employee.Salary' = '65000'
        'Employee.Status' = '0'
        'Employee.CompanyId' = '5ED98643-37E3-E3D0-83A9-3A1CC380A120'
        'Employee.SocialSecurityNumber' = '987-654-321'
        'Employee.ZipCode' = 'H2X 3Y7'
        'Employee.Country' = 'Canada'
    }
    
    Write-Host "   ✓ Test data prepared for: $($testEmployee['Employee.FirstName']) $($testEmployee['Employee.LastName'])" -ForegroundColor Green
    
    # Submit form
    Write-Host "3. Submitting employee creation form..." -ForegroundColor Cyan
    $response = Invoke-WebRequest -Uri "https://localhost:44329/Employees/Create" -Method POST -Body $testEmployee -WebSession $session
    
    Write-Host "   Response Status: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "   Response URL: $($response.BaseResponse.ResponseUri)" -ForegroundColor Gray
    
    # Check result
    if ($response.BaseResponse.ResponseUri.ToString().Contains("/Companies/Index")) {
        Write-Host "   ✓ SUCCESS: Redirected to Companies/Index - Employee likely created!" -ForegroundColor Green
    } elseif ($response.Content.Contains("Employee created")) {
        Write-Host "   ✓ SUCCESS: Success message found!" -ForegroundColor Green
    } elseif ($response.Content.Contains("validation") -or $response.Content.Contains("error")) {
        Write-Host "   ⚠ VALIDATION: Form has validation issues" -ForegroundColor Yellow
    } else {
        Write-Host "   ? UNCLEAR: Response received but status unclear" -ForegroundColor Yellow
    }
    
} catch {
    Write-Host "✗ ERROR: $($_.Exception.Message)" -ForegroundColor Red
}

# Check database
Write-Host "4. Checking database for new employee..." -ForegroundColor Cyan
try {
    $dbResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q "SELECT COUNT(*) FROM AppEmployees WHERE FirstName = 'Marie' AND LastName = 'Dupont'" -h -1
    if ($dbResult -and $dbResult.Trim() -gt 0) {
        Write-Host "   ✓ Employee found in database!" -ForegroundColor Green
    } else {
        Write-Host "   ✗ Employee not found in database" -ForegroundColor Red
    }
} catch {
    Write-Host "   ✗ Database check failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nTest completed." -ForegroundColor Green