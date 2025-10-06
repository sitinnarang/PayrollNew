# Test employee form submission with all required fields
$baseUrl = "https://localhost:44329"

# Bypass SSL certificate validation
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

Write-Host "=== Testing Employee Creation Form ===" -ForegroundColor Cyan

# Get the form first to extract anti-forgery token
$formUrl = "$baseUrl/Employees/Create"
Write-Host "Getting form: $formUrl" -ForegroundColor Yellow

try {
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $formResponse = Invoke-WebRequest -Uri $formUrl -Method GET -SessionVariable session

    # Extract anti-forgery token
    $antiForgeryToken = ""
    if ($formResponse.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
        $antiForgeryToken = $matches[1]
        Write-Host "Anti-forgery token extracted: $($antiForgeryToken.Substring(0, 10))..." -ForegroundColor Green
    }

    # Prepare form data with all required fields
    $formData = @{
        '__RequestVerificationToken' = $antiForgeryToken
        'Employee.FirstName' = 'John'
        'Employee.LastName' = 'TestEmployee'
        'Employee.Email' = 'john.test@company.com'
        'Employee.EmployeeId' = 'EMP-001'
        'Employee.Department' = 'IT'
        'Employee.Position' = 'Developer'
        'Employee.Salary' = '75000'
        'Employee.HireDate' = '2024-10-06'
        'Employee.Status' = '1'  # Active
        'Employee.CompanyId' = 'edd1aebc-84bc-cc06-dacb-3a1cc444d0b2'  # DBS Bank ID
    }

    Write-Host "Submitting form with data:" -ForegroundColor Yellow
    $formData.GetEnumerator() | Where-Object { $_.Key -ne '__RequestVerificationToken' } | ForEach-Object {
        Write-Host "  $($_.Key): $($_.Value)" -ForegroundColor Gray
    }

    # Submit the form
    $submitResponse = Invoke-WebRequest -Uri $formUrl -Method POST -Body $formData -WebSession $session -MaximumRedirection 0 -ErrorAction SilentlyContinue

    Write-Host "`nResponse Status: $($submitResponse.StatusCode)" -ForegroundColor $(if($submitResponse.StatusCode -eq 302) { 'Green' } else { 'Red' })
    
    if ($submitResponse.Headers.Location) {
        Write-Host "Redirect Location: $($submitResponse.Headers.Location)" -ForegroundColor Green
    }

    if ($submitResponse.StatusCode -eq 200) {
        # Form returned with validation errors
        if ($submitResponse.Content -match 'text-danger.*?>(.*?)<') {
            Write-Host "Validation errors found in response" -ForegroundColor Red
        }
        Write-Host "Form content length: $($submitResponse.Content.Length)" -ForegroundColor Yellow
    }

} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Cyan