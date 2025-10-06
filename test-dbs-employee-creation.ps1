#!/usr/bin/env pwsh

Write-Host "=== Testing DBS User Employee Creation ===" -ForegroundColor Cyan

# Trust all SSL certificates for testing
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Step 1: Login as DBS user
    Write-Host "Step 1: Logging in as DBS user..." -ForegroundColor Yellow
    
    $loginUrl = "https://localhost:44329/Account/Login"
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    
    # Get login form to extract anti-forgery token
    $loginForm = Invoke-WebRequest -Uri $loginUrl -Method Get -WebSession $session
    $loginToken = ""
    if ($loginForm.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
        $loginToken = $matches[1]
        Write-Host "Login token extracted: $($loginToken.Substring(0, 10))..." -ForegroundColor Green
    } else {
        Write-Host "Failed to extract login token" -ForegroundColor Red
        exit 1
    }
    
    # Login with DBS credentials
    $loginData = @{
        '__RequestVerificationToken' = $loginToken
        'UserNameOrEmailAddress' = 'DBS'
        'Password' = 'DBSPass123!'
        'RememberMe' = 'false'
    }
    
    $loginResponse = Invoke-WebRequest -Uri $loginUrl -Method Post -Body $loginData -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue
    
    if ($loginResponse.StatusCode -eq 302 -or $loginResponse.Headers.Location) {
        Write-Host "✅ LOGIN SUCCESS: DBS user logged in successfully" -ForegroundColor Green
    } else {
        Write-Host "❌ LOGIN FAILED: Status $($loginResponse.StatusCode)" -ForegroundColor Red
        Write-Host "Response: $($loginResponse.Content.Substring(0, 500))" -ForegroundColor Gray
        exit 1
    }
    
    # Step 2: Get employee creation form
    Write-Host "Step 2: Getting employee creation form..." -ForegroundColor Yellow
    
    $createUrl = "https://localhost:44329/Employees/Create"
    $formResponse = Invoke-WebRequest -Uri $createUrl -Method Get -WebSession $session
    
    if ($formResponse.StatusCode -eq 200) {
        Write-Host "✅ FORM ACCESS: Employee creation form accessible" -ForegroundColor Green
    } else {
        Write-Host "❌ FORM ACCESS DENIED: Status $($formResponse.StatusCode)" -ForegroundColor Red
        if ($formResponse.Content -match "access.*denied|unauthorized|permission") {
            Write-Host "🚫 PERMISSION ISSUE: DBS user lacks permission to access employee creation" -ForegroundColor Red
        }
        exit 1
    }
    
    # Extract anti-forgery token from creation form
    $antiForgeryToken = ""
    if ($formResponse.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
        $antiForgeryToken = $matches[1]
        Write-Host "Form token extracted: $($antiForgeryToken.Substring(0, 10))..." -ForegroundColor Green
    } else {
        Write-Host "Failed to extract form token" -ForegroundColor Red
        exit 1
    }
    
    # Get DBS company ID
    Write-Host "Step 3: Getting DBS company ID..." -ForegroundColor Yellow
    $dbsCompanyQuery = "SELECT Id FROM AppCompanies WHERE Name = 'DBS Bank'"
    $dbsCompanyResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q $dbsCompanyQuery -h -1 -W
    $dbsCompanyId = ($dbsCompanyResult | Where-Object { $_ -match "[0-9A-F-]+" }).Trim()
    
    if ($dbsCompanyId) {
        Write-Host "✅ COMPANY ID: $dbsCompanyId" -ForegroundColor Green
    } else {
        Write-Host "❌ COMPANY ID NOT FOUND" -ForegroundColor Red
        exit 1
    }
    
    # Step 4: Submit employee creation form
    Write-Host "Step 4: Creating employee..." -ForegroundColor Yellow
    
    $formData = @{
        '__RequestVerificationToken' = $antiForgeryToken
        'Employee.FirstName' = 'John'
        'Employee.LastName' = 'DBSEmployee'
        'Employee.Email' = 'john.dbsemployee@dbs.com'
        'Employee.EmployeeId' = 'DBS-EMP-001'
        'Employee.Department' = 'IT'
        'Employee.Position' = 'Developer'
        'Employee.Salary' = '75000'
        'Employee.HireDate' = '2024-10-06'
        'Employee.Status' = '1'  # Active
        'Employee.CompanyId' = $dbsCompanyId
    }
    
    Write-Host "Submitting employee data:" -ForegroundColor Cyan
    $formData.GetEnumerator() | Where-Object { $_.Key -ne '__RequestVerificationToken' } | ForEach-Object { 
        Write-Host "  $($_.Key): $($_.Value)" -ForegroundColor Gray
    }
    
    $createResponse = Invoke-WebRequest -Uri $createUrl -Method Post -Body $formData -WebSession $session
    
    # Check response
    if ($createResponse.StatusCode -eq 200) {
        if ($createResponse.Content -match "validation-summary|field-validation-error|alert-danger") {
            Write-Host "❌ VALIDATION ERRORS: Employee creation failed with validation errors" -ForegroundColor Red
        } elseif ($createResponse.Content -match "Employee.*created|success|created successfully") {
            Write-Host "✅ SUCCESS: Employee created successfully!" -ForegroundColor Green
        } elseif ($createResponse.Content -match "unauthorized|access.*denied|permission") {
            Write-Host "❌ AUTHORIZATION: DBS user lacks permission to create employees" -ForegroundColor Red
        } else {
            Write-Host "⚠️ UNKNOWN RESULT: Response received but outcome unclear" -ForegroundColor Yellow
            Write-Host "Response length: $($createResponse.Content.Length)" -ForegroundColor Gray
        }
    } else {
        Write-Host "❌ ERROR: HTTP Status $($createResponse.StatusCode)" -ForegroundColor Red
    }
    
    # Step 5: Verify in database
    Write-Host "Step 5: Checking database..." -ForegroundColor Yellow
    
    $employeeQuery = "SELECT Id, FirstName, LastName, EmployeeId FROM AppEmployees WHERE EmployeeId = 'DBS-EMP-001'"
    $employeeResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q $employeeQuery
    
    if ($employeeResult -match "DBS-EMP-001") {
        Write-Host "SUCCESS: Employee DBS-EMP-001 found in database!" -ForegroundColor Green
        Write-Host "CONCLUSION: DBS user CAN create employees successfully!" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Employee not found in database" -ForegroundColor Red
        Write-Host "CONCLUSION: DBS user CANNOT create employees (permission or other issue)" -ForegroundColor Red
    }

} catch {
    Write-Host "EXCEPTION: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Cyan