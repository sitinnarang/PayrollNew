#!/usr/bin/env pwsh

Write-Host "=== DBS User Employee Creation Test ===" -ForegroundColor Cyan

# Add .NET assemblies and configure SSL
Add-Type -AssemblyName System.Net.Http
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = { $true }

$baseUrl = "https://localhost:44329"

try {
    # Create HTTP client with cookie container
    $handler = New-Object System.Net.Http.HttpClientHandler
    $handler.CookieContainer = New-Object System.Net.CookieContainer
    $handler.ServerCertificateCustomValidationCallback = { $true }
    
    $client = New-Object System.Net.Http.HttpClient($handler)
    $client.DefaultRequestHeaders.Add("User-Agent", "PowerShell Test Client")

    Write-Host "Step 1: Getting login page..." -ForegroundColor Yellow
    
    # Get login page
    $loginPageResponse = $client.GetAsync("$baseUrl/Account/Login").Result
    $loginPageContent = $loginPageResponse.Content.ReadAsStringAsync().Result
    
    if ($loginPageResponse.IsSuccessStatusCode) {
        Write-Host "SUCCESS: Login page loaded" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Cannot load login page" -ForegroundColor Red
        return
    }
    
    # Extract anti-forgery token
    $tokenMatch = [regex]::Match($loginPageContent, '__RequestVerificationToken.*?value="([^"]+)"')
    if ($tokenMatch.Success) {
        $loginToken = $tokenMatch.Groups[1].Value
        Write-Host "SUCCESS: Login token extracted" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Cannot find login token" -ForegroundColor Red
        return
    }

    Write-Host "Step 2: Logging in as DBS user..." -ForegroundColor Yellow
    
    # Prepare login data
    $loginParams = @(
        "__RequestVerificationToken=$loginToken",
        "UserNameOrEmailAddress=DBS", 
        "Password=DBSPass123!",
        "RememberMe=false"
    )
    $loginData = $loginParams -join "&"
    
    # Login request
    $loginContent = New-Object System.Net.Http.StringContent($loginData, [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded")
    $loginResponse = $client.PostAsync("$baseUrl/Account/Login", $loginContent).Result
    
    # Check if login was successful (redirect or success)
    if ($loginResponse.IsSuccessStatusCode -or $loginResponse.StatusCode -eq "Redirect") {
        Write-Host "SUCCESS: DBS user logged in" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Login failed - Status: $($loginResponse.StatusCode)" -ForegroundColor Red
        return
    }

    Write-Host "Step 3: Accessing employee creation form..." -ForegroundColor Yellow
    
    # Get employee creation form
    $createFormResponse = $client.GetAsync("$baseUrl/Employees/Create").Result
    $createFormContent = $createFormResponse.Content.ReadAsStringAsync().Result
    
    if ($createFormResponse.IsSuccessStatusCode) {
        Write-Host "SUCCESS: Employee creation form accessible" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Cannot access employee form - Status: $($createFormResponse.StatusCode)" -ForegroundColor Red
        
        # Check if it's a permission issue
        if ($createFormContent -match "unauthorized|access.*denied|permission|login") {
            Write-Host "CAUSE: Permission or authentication issue" -ForegroundColor Yellow
        }
        return
    }
    
    # Extract form token
    $formTokenMatch = [regex]::Match($createFormContent, '__RequestVerificationToken.*?value="([^"]+)"')
    if ($formTokenMatch.Success) {
        $formToken = $formTokenMatch.Groups[1].Value
        Write-Host "SUCCESS: Form token extracted" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Cannot find form token" -ForegroundColor Red
        return
    }

    Write-Host "Step 4: Creating employee..." -ForegroundColor Yellow
    
    # Get DBS company ID
    $companyQuery = "SELECT Id FROM AppCompanies WHERE Name = 'DBS Bank'"
    $companyResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q $companyQuery -h -1 -W
    $companyId = ($companyResult | Where-Object { $_ -match "[0-9A-F-]+" } | Select-Object -First 1).Trim()
    
    $employeeId = "DBS-WEB-" + (Get-Date -Format "MMddHHmmss")
    
    # Prepare employee data
    $employeeParams = @(
        "__RequestVerificationToken=$formToken",
        "Employee.FirstName=John",
        "Employee.LastName=WebTest",
        "Employee.Email=john.webtest@dbs.com",
        "Employee.EmployeeId=$employeeId",
        "Employee.Department=IT",
        "Employee.Position=Developer", 
        "Employee.Salary=75000",
        "Employee.HireDate=2024-10-06",
        "Employee.Status=1",
        "Employee.CompanyId=$companyId"
    )
    $employeeData = $employeeParams -join "&"
    
    Write-Host "Creating employee: $employeeId" -ForegroundColor Cyan
    
    # Submit employee creation
    $employeeContent = New-Object System.Net.Http.StringContent($employeeData, [System.Text.Encoding]::UTF8, "application/x-www-form-urlencoded")
    $employeeResponse = $client.PostAsync("$baseUrl/Employees/Create", $employeeContent).Result
    $employeeResponseContent = $employeeResponse.Content.ReadAsStringAsync().Result
    
    Write-Host "Step 5: Checking response..." -ForegroundColor Yellow
    
    if ($employeeResponse.IsSuccessStatusCode) {
        Write-Host "SUCCESS: Employee creation request completed" -ForegroundColor Green
        
        # Check for validation errors
        if ($employeeResponseContent -match "validation-summary|field-validation-error|alert-danger") {
            Write-Host "WARNING: Validation errors detected in response" -ForegroundColor Yellow
        } else {
            Write-Host "SUCCESS: No validation errors detected" -ForegroundColor Green
        }
    } else {
        Write-Host "ERROR: Employee creation failed - Status: $($employeeResponse.StatusCode)" -ForegroundColor Red
    }

    Write-Host "Step 6: Verifying in database..." -ForegroundColor Yellow
    
    Start-Sleep -Seconds 2
    
    # Check database
    $verifyQuery = "SELECT Id, EmployeeId, FirstName, LastName, Email FROM AppEmployees WHERE EmployeeId = '$employeeId'"
    $verifyResult = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q $verifyQuery
    
    if ($verifyResult -match $employeeId) {
        Write-Host "SUCCESS: Employee '$employeeId' found in database!" -ForegroundColor Green
        Write-Host "CONCLUSION: DBS user CAN create employees through web interface!" -ForegroundColor Green
        
        # Show created employee
        Write-Host "Employee created:" -ForegroundColor Cyan
        $verifyResult | ForEach-Object { 
            if ($_ -notmatch "---" -and $_ -notmatch "rows affected" -and $_.Trim() -ne "") { 
                Write-Host "  $_" -ForegroundColor Gray 
            } 
        }
        
    } else {
        Write-Host "ERROR: Employee '$employeeId' not found in database" -ForegroundColor Red
        Write-Host "CONCLUSION: Employee creation through web interface failed" -ForegroundColor Red
    }
    
} catch {
    Write-Host "EXCEPTION: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.InnerException) {
        Write-Host "Inner Exception: $($_.Exception.InnerException.Message)" -ForegroundColor Red
    }
} finally {
    if ($client) { $client.Dispose() }
    if ($handler) { $handler.Dispose() }
}

Write-Host ""
Write-Host "=== Test Complete ===" -ForegroundColor Cyan