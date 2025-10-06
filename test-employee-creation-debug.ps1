# Simple Employee Creation Test Script
Write-Host "Testing Employee Creation Functionality..." -ForegroundColor Green

# Check if application is running
$port = 44329
$isRunning = $false

try {
    $response = Invoke-WebRequest -Uri "https://localhost:$port" -Method GET -SkipCertificateCheck -TimeoutSec 5
    $isRunning = $true
    Write-Host "✓ Application is running at https://localhost:$port" -ForegroundColor Green
} catch {
    Write-Host "✗ Application is not running at https://localhost:$port" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
}

if ($isRunning) {
    # Test employee creation page access
    try {
        $createPageResponse = Invoke-WebRequest -Uri "https://localhost:$port/Employees/Create" -Method GET -SkipCertificateCheck -TimeoutSec 10
        Write-Host "✓ Employee creation page is accessible" -ForegroundColor Green
        
        # Check if the page contains the form
        if ($createPageResponse.Content -like "*employeeForm*") {
            Write-Host "✓ Employee form is present on the page" -ForegroundColor Green
        } else {
            Write-Host "✗ Employee form is not found on the page" -ForegroundColor Yellow
        }
        
        # Check if Canadian validation script is loaded
        if ($createPageResponse.Content -like "*canadian-employee-validation.js*") {
            Write-Host "✓ Canadian validation script is included" -ForegroundColor Green
        } else {
            Write-Host "✗ Canadian validation script is not included" -ForegroundColor Yellow
        }
        
        # Check if CompanyId field is present
        if ($createPageResponse.Content -like "*Employee.CompanyId*") {
            Write-Host "✓ CompanyId field is present in the form" -ForegroundColor Green
        } else {
            Write-Host "✗ CompanyId field is missing from the form" -ForegroundColor Red
        }
        
    } catch {
        Write-Host "✗ Failed to access employee creation page" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    # Test database connectivity by checking companies
    try {
        Write-Host "`nChecking database connectivity..." -ForegroundColor Cyan
        $companies = sqlcmd -S ".\SQLEXPRESS" -d "PayrollPro" -E -Q "SELECT COUNT(*) as CompanyCount FROM AppCompanies" -h -1
        if ($companies -and $companies.Trim() -match '^\d+$') {
            Write-Host "✓ Database is accessible with $($companies.Trim()) companies" -ForegroundColor Green
        } else {
            Write-Host "✗ Database query failed or returned invalid result" -ForegroundColor Red
        }
    } catch {
        Write-Host "✗ Database connectivity failed" -ForegroundColor Red
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    }
} else {
    Write-Host "`nApplication is not running. Starting application..." -ForegroundColor Cyan
    Write-Host "Please wait for application to start, then run this script again." -ForegroundColor Yellow
}

Write-Host "`nTest completed." -ForegroundColor Green