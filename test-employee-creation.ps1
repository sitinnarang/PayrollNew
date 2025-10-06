# Test Employee Creation Functionality
Write-Host "Testing Employee Creation..." -ForegroundColor Green

# Bypass SSL certificate validation for localhost testing
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

# First, let's verify the DBS company exists
try {
    Write-Host "1. Checking if DBS Company exists..." -ForegroundColor Yellow
    
    $companyCheckUrl = "https://localhost:44329/api/app/company"
    $response = Invoke-RestMethod -Uri $companyCheckUrl -Method Get -ContentType 'application/json'
    
    $dbsCompany = $response.items | Where-Object { $_.name -eq 'DBS Bank' -or $_.code -eq 'DBS' }
    
    if ($dbsCompany) {
        Write-Host "✓ DBS Company found: $($dbsCompany.name) (ID: $($dbsCompany.id))" -ForegroundColor Green
        $companyId = $dbsCompany.id
    } else {
        Write-Host "✗ DBS Company not found. Creating a test company..." -ForegroundColor Red
        
        # Create a test company
        $companyData = @{
            Name = "Test Company"
            Code = "TEST"
            Description = "Test Company for Employee Creation"
            Address = "123 Test Street"
            City = "Test City"
            State = "Test State"
            ZipCode = "12345"
            Country = "USA"
            Phone = "+1-555-0123"
            Email = "test@test.com"
            Website = "https://test.com"
            TaxId = "TEST123"
            RegistrationNumber = "REG123"
            EstablishedDate = "2024-01-01T00:00:00Z"
        } | ConvertTo-Json
        
        $createCompanyUrl = "https://localhost:44329/api/app/company"
        $newCompany = Invoke-RestMethod -Uri $createCompanyUrl -Method Post -Body $companyData -ContentType 'application/json'
        $companyId = $newCompany.id
        Write-Host "✓ Test company created (ID: $companyId)" -ForegroundColor Green
    }
    
    # Now test employee creation
    Write-Host "2. Testing Employee Creation..." -ForegroundColor Yellow
    
    $employeeData = @{
        FirstName = "John"
        LastName = "Doe"
        Email = "john.doe@test.com"
        Phone = "+1-555-0456"
        EmployeeId = "EMP001"
        Department = "IT"
        Position = "Software Developer"
        HireDate = "2024-10-06T00:00:00Z"
        Salary = 75000
        Status = 0  # Active
        CompanyId = $companyId
        Notes = "Test employee created via API"
        Address = "456 Employee Street"
        City = "Employee City"
        State = "Employee State"
        ZipCode = "54321"
        Country = "USA"
        DateOfBirth = "1990-01-15T00:00:00Z"
        EmergencyContactName = "Jane Doe"
        EmergencyContactPhone = "+1-555-0789"
        DisplayName = "John D."
        Gender = "Male"
        MobilePhone = "+1-555-0101"
    } | ConvertTo-Json
    
    $createEmployeeUrl = "https://localhost:44329/api/app/employee"
    $newEmployee = Invoke-RestMethod -Uri $createEmployeeUrl -Method Post -Body $employeeData -ContentType 'application/json'
    
    Write-Host "✓ Employee created successfully!" -ForegroundColor Green
    Write-Host "   Employee ID: $($newEmployee.employeeId)" -ForegroundColor Cyan
    Write-Host "   Full Name: $($newEmployee.fullName)" -ForegroundColor Cyan
    Write-Host "   Email: $($newEmployee.email)" -ForegroundColor Cyan
    Write-Host "   Department: $($newEmployee.department)" -ForegroundColor Cyan
    Write-Host "   Position: $($newEmployee.position)" -ForegroundColor Cyan
    
    Write-Host "3. Testing Employee Retrieval..." -ForegroundColor Yellow
    $getEmployeeUrl = "https://localhost:44329/api/app/employee/$($newEmployee.id)"
    $retrievedEmployee = Invoke-RestMethod -Uri $getEmployeeUrl -Method Get -ContentType 'application/json'
    
    Write-Host "✓ Employee retrieved successfully!" -ForegroundColor Green
    Write-Host "   Full Name: $($retrievedEmployee.fullName)" -ForegroundColor Cyan
    Write-Host "   Date of Birth: $($retrievedEmployee.dateOfBirth)" -ForegroundColor Cyan
    Write-Host "   Emergency Contact: $($retrievedEmployee.emergencyContactName)" -ForegroundColor Cyan
    
    Write-Host "`nEmployee Creation Test PASSED! ✓" -ForegroundColor Green
}
catch {
    Write-Host "✗ Test Failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Response Details: $($_.ErrorDetails.Message)" -ForegroundColor Red
    Write-Host "`nEmployee Creation Test FAILED! ✗" -ForegroundColor Red
}