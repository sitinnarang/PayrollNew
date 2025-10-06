#!/usr/bin/env pwsh

Write-Host "=== Testing DBS User Employee Creation ===" -ForegroundColor Cyan

# Trust all SSL certificates for testing
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # Get DBS company ID first
    Write-Host "Step 1: Getting DBS company ID..." -ForegroundColor Yellow
    $dbsCompanyQuery = "SELECT Id FROM AppCompanies WHERE Name = 'DBS Bank'"
    $dbsCompanyResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q $dbsCompanyQuery -h -1 -W
    $dbsCompanyId = ($dbsCompanyResult | Where-Object { $_ -match "[0-9A-F-]+" } | Select-Object -First 1).Trim()
    
    if ($dbsCompanyId) {
        Write-Host "SUCCESS: DBS Company ID = $dbsCompanyId" -ForegroundColor Green
    } else {
        Write-Host "ERROR: DBS Company not found" -ForegroundColor Red
        exit 1
    }

    # Step 2: Login as DBS user
    Write-Host "Step 2: Logging in as DBS user..." -ForegroundColor Yellow
    
    $loginUrl = "https://localhost:44329/Account/Login"
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    
    # Get login form
    $loginForm = Invoke-WebRequest -Uri $loginUrl -Method Get -WebSession $session
    $loginToken = ""
    if ($loginForm.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
        $loginToken = $matches[1]
        Write-Host "Login token extracted successfully" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Failed to extract login token" -ForegroundColor Red
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
    
    if ($loginResponse.StatusCode -eq 302) {
        Write-Host "SUCCESS: DBS user logged in successfully" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Login failed with status $($loginResponse.StatusCode)" -ForegroundColor Red
        exit 1
    }
    
    # Step 3: Access employee creation form
    Write-Host "Step 3: Accessing employee creation form..." -ForegroundColor Yellow
    
    $createUrl = "https://localhost:44329/Employees/Create"
    $formResponse = Invoke-WebRequest -Uri $createUrl -Method Get -WebSession $session
    
    if ($formResponse.StatusCode -eq 200) {
        Write-Host "SUCCESS: Employee creation form accessible" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Cannot access form - Status $($formResponse.StatusCode)" -ForegroundColor Red
        exit 1
    }
    
    # Extract anti-forgery token
    $antiForgeryToken = ""
    if ($formResponse.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
        $antiForgeryToken = $matches[1]
        Write-Host "Form token extracted successfully" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Failed to extract form token" -ForegroundColor Red
        exit 1
    }
    
    # Step 4: Create employee
    Write-Host "Step 4: Creating employee..." -ForegroundColor Yellow
    
    $employeeId = "DBS-TEST-" + (Get-Date -Format "MMddHHmm")
    
    $formData = @{
        '__RequestVerificationToken' = $antiForgeryToken
        'Employee.FirstName' = 'John'
        'Employee.LastName' = 'DBSEmployee'
        'Employee.Email' = "john.test.$(Get-Date -Format 'MMddHHmm')@dbs.com"
        'Employee.EmployeeId' = $employeeId
        'Employee.Department' = 'IT'
        'Employee.Position' = 'Developer'
        'Employee.Salary' = '75000'
        'Employee.HireDate' = '2024-10-06'
        'Employee.Status' = '1'  # Active
        'Employee.CompanyId' = $dbsCompanyId
    }
    
    Write-Host "Creating employee with ID: $employeeId" -ForegroundColor Cyan
    
    $createResponse = Invoke-WebRequest -Uri $createUrl -Method Post -Body $formData -WebSession $session
    
    # Step 5: Analyze response
    Write-Host "Step 5: Analyzing response..." -ForegroundColor Yellow
    
    if ($createResponse.StatusCode -eq 200) {
        if ($createResponse.Content -match "validation-summary|field-validation-error|alert-danger") {
            Write-Host "ERROR: Validation errors in response" -ForegroundColor Red
            # Try to extract specific errors
            if ($createResponse.Content -match '<div class="validation-summary-errors"[^>]*>(.*?)</div>') {
                Write-Host "Validation errors found in HTML" -ForegroundColor Red
            }
        } elseif ($createResponse.Content -match "unauthorized|access.*denied|permission") {
            Write-Host "ERROR: Access denied - insufficient permissions" -ForegroundColor Red
        } else {
            Write-Host "SUCCESS: Employee creation submitted (Status 200)" -ForegroundColor Green
        }
    } else {
        Write-Host "ERROR: HTTP Status $($createResponse.StatusCode)" -ForegroundColor Red
    }
    
    # Step 6: Verify in database
    Write-Host "Step 6: Verifying in database..." -ForegroundColor Yellow
    
    Start-Sleep -Seconds 2  # Give database time to update
    
    $verifyQuery = "SELECT Id, FirstName, LastName, EmployeeId, Status FROM AppEmployees WHERE EmployeeId = '$employeeId'"
    $verifyResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q $verifyQuery
    
    if ($verifyResult -match $employeeId) {
        Write-Host "SUCCESS: Employee '$employeeId' found in database!" -ForegroundColor Green
        Write-Host "RESULT: DBS user CAN create employees successfully!" -ForegroundColor Green
        
        # Show employee details
        Write-Host "Employee details:" -ForegroundColor Cyan
        $verifyResult | ForEach-Object { if ($_ -notmatch "---" -and $_ -notmatch "rows affected" -and $_.Trim() -ne "") { Write-Host "  $_" -ForegroundColor Gray } }
        
    } else {
        Write-Host "ERROR: Employee '$employeeId' not found in database" -ForegroundColor Red
        Write-Host "RESULT: Employee creation failed" -ForegroundColor Red
    }

} catch {
    Write-Host "EXCEPTION: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "RESULT: Test failed due to exception" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Cyan