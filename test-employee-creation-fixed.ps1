# Test Employee Creation with Real Form Submission
Write-Host "Testing Employee Creation with Fixed JavaScript..." -ForegroundColor Green

# Disable SSL validation for testing
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

try {
    # First get the Create page to get the anti-forgery token
    $createPageResponse = Invoke-WebRequest -Uri "https://localhost:44329/Employees/Create" -Method GET -SessionVariable session
    
    if ($createPageResponse.StatusCode -eq 200) {
        Write-Host "✓ Employee creation page loaded successfully" -ForegroundColor Green
        
        # Extract anti-forgery token
        $antiForgeryToken = ""
        if ($createPageResponse.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
            $antiForgeryToken = $matches[1]
            Write-Host "✓ Anti-forgery token extracted: $($antiForgeryToken.Substring(0, 20))..." -ForegroundColor Green
        } else {
            Write-Host "✗ Could not extract anti-forgery token" -ForegroundColor Yellow
        }
        
        # Check if form elements are present
        $formChecks = @(
            @{Name="Employee Form"; Pattern="employeeForm"},
            @{Name="CompanyId Hidden Field"; Pattern="Employee\.CompanyId"},
            @{Name="FirstName Field"; Pattern="Employee\.FirstName"},
            @{Name="LastName Field"; Pattern="Employee\.LastName"},
            @{Name="Submit Button"; Pattern="Create Employee"}
        )
        
        foreach ($check in $formChecks) {
            if ($createPageResponse.Content -like "*$($check.Pattern)*") {
                Write-Host "✓ $($check.Name) found" -ForegroundColor Green
            } else {
                Write-Host "✗ $($check.Name) missing" -ForegroundColor Red
            }
        }
        
        # Test form data (Canadian employee)
        $formData = @{
            '__RequestVerificationToken' = $antiForgeryToken
            'Employee.FirstName' = 'Jean-Baptiste'
            'Employee.LastName' = 'Tremblay'
            'Employee.Email' = 'jean.tremblay@test.ca'
            'Employee.Phone' = '514-555-1234'
            'Employee.EmployeeId' = 'EMP-001-CA'
            'Employee.Department' = 'Engineering'
            'Employee.Position' = 'Software Developer'
            'Employee.HireDate' = (Get-Date).ToString('yyyy-MM-dd')
            'Employee.Salary' = '75000'
            'Employee.Status' = '0'  # Active
            'Employee.CompanyId' = '5ED98643-37E3-E3D0-83A9-3A1CC380A120'
            'Employee.SocialSecurityNumber' = '123-456-789'
            'Employee.Address' = '123 Maple Street'
            'Employee.City' = 'Montreal'
            'Employee.State' = 'Quebec'
            'Employee.ZipCode' = 'H1A 1A1'
            'Employee.Country' = 'Canada'
        }
        
        Write-Host "`nTesting employee creation with Canadian test data..." -ForegroundColor Cyan
        
        # Submit the form
        $postResponse = Invoke-WebRequest -Uri "https://localhost:44329/Employees/Create" -Method POST -Body $formData -WebSession $session
        
        if ($postResponse.StatusCode -eq 200) {
            if ($postResponse.BaseResponse.ResponseUri.AbsolutePath -eq "/Companies/Index") {
                Write-Host "✓ SUCCESS: Employee created successfully! Redirected to Companies/Index" -ForegroundColor Green
            } elseif ($postResponse.Content -like "*successMessage*" -or $postResponse.Content -like "*Employee created*") {
                Write-Host "✓ SUCCESS: Employee appears to be created (success message found)" -ForegroundColor Green
            } elseif ($postResponse.Content -like "*Validation*" -or $postResponse.Content -like "*error*") {
                Write-Host "⚠ Form submission returned validation errors" -ForegroundColor Yellow
                # Extract error messages
                if ($postResponse.Content -match 'validation-summary.*?<ul>(.*?)</ul>') {
                    $errors = $matches[1]
                    Write-Host "Errors: $errors" -ForegroundColor Red
                }
            } else {
                Write-Host "⚠ Form submitted but response unclear. Checking content..." -ForegroundColor Yellow
                if ($postResponse.Content.Length -gt 0) {
                    Write-Host "Response received ($(($postResponse.Content).Length) chars)" -ForegroundColor Gray
                }
            }
        } else {
            Write-Host "✗ Form submission failed with status: $($postResponse.StatusCode)" -ForegroundColor Red
        }
        
    } else {
        Write-Host "✗ Failed to load employee creation page: $($createPageResponse.StatusCode)" -ForegroundColor Red
    }
    
} catch {
    Write-Host "✗ Test failed with error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Response status: $($_.Exception.Response.StatusCode)" -ForegroundColor Gray
    }
}

# Check database to see if employee was created
try {
    Write-Host "`nChecking database for created employee..." -ForegroundColor Cyan
    $employeeCheck = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q "SELECT TOP 1 Id, FirstName, LastName, Email FROM AppEmployees ORDER BY CreationTime DESC" -h -1
    if ($employeeCheck -and $employeeCheck -like "*Jean-Baptiste*") {
        Write-Host "✓ Employee found in database!" -ForegroundColor Green
        Write-Host "Latest employee: $employeeCheck" -ForegroundColor Gray
    } else {
        Write-Host "⚠ No matching employee found in database" -ForegroundColor Yellow
        if ($employeeCheck) {
            Write-Host "Latest employee: $employeeCheck" -ForegroundColor Gray
        }
    }
} catch {
    Write-Host "✗ Database check failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`nEmployee Creation Test Complete" -ForegroundColor Green