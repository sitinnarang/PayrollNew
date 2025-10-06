# Test script to verify user creation in PayrollPro
param(
    [string]$BaseUrl = "https://localhost:44329",
    [string]$Username = "testuser001",
    [string]$Email = "testuser001@example.com",
    [string]$Password = "1q2w3E*",
    [string]$CompanyName = "Test Company Ltd"
)

Write-Host "Testing PayrollPro User & Company Creation" -ForegroundColor Green
Write-Host "Base URL: $BaseUrl" -ForegroundColor Yellow

try {
    # First, get the anti-forgery token
    Write-Host "Getting anti-forgery token..." -ForegroundColor Cyan
    
    # Skip certificate validation for localhost
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
    
    $getResponse = Invoke-WebRequest -Uri "$BaseUrl/Companies/CreateCompany" -UseBasicParsing
    
    $tokenMatch = $getResponse.Content | Select-String -Pattern 'name="__RequestVerificationToken".*?value="([^"]+)"'
    if ($tokenMatch.Matches.Count -eq 0) {
        Write-Host "Could not find anti-forgery token" -ForegroundColor Red
        exit 1
    }
    
    $token = $tokenMatch.Matches[0].Groups[1].Value
    Write-Host "Got token: $($token.Substring(0, 20))..." -ForegroundColor Green
    
    # Create the form data - include all required fields
    $formData = @{
        "__RequestVerificationToken" = $token
        
        # User Registration fields
        "UserRegistration.Username" = $Username
        "UserRegistration.Email" = $Email
        "UserRegistration.Password" = $Password
        "UserRegistration.ConfirmPassword" = $Password
        
        # Company required fields
        "Company.Name" = $CompanyName
        "Company.Code" = "TC001"
        "Company.Description" = "Test company for user creation test"
        "Company.EstablishedDate" = "2024-01-01"
        "Company.Address" = "123 Test Street"
        "Company.City" = "Test City"
        "Company.State" = "Test State"
        "Company.ZipCode" = "12345"
        "Company.Country" = "Test Country"
        "Company.Phone" = "555-0123"
        "Company.Email" = "info@testcompany.com"
        "Company.Website" = "https://testcompany.com"
        "Company.TaxId" = "TAX123456"
        "Company.RegistrationNumber" = "REG789012"
        "Company.LogoUrl" = ""
        "Company.IsActive" = "true"
    }
    
    Write-Host "Submitting form data..." -ForegroundColor Cyan
    
    # Submit the form
    try {
        $postResponse = Invoke-WebRequest -Uri "$BaseUrl/Companies/CreateCompany" -Method POST -Body $formData -UseBasicParsing -ContentType "application/x-www-form-urlencoded"
        
        Write-Host "Response Status: $($postResponse.StatusCode)" -ForegroundColor Yellow
        Write-Host "Response Content:" -ForegroundColor Yellow
        Write-Host $postResponse.Content -ForegroundColor White
    }
    catch {
        Write-Host "HTTP Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $responseStream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($responseStream)
            $responseBody = $reader.ReadToEnd()
            Write-Host "Response Body:" -ForegroundColor Yellow
            Write-Host $responseBody -ForegroundColor White
        }
        throw
    }
    
    if ($postResponse.StatusCode -eq 200) {
        Write-Host "Request completed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Request failed with status: $($postResponse.StatusCode)" -ForegroundColor Red
    }
    
} catch {
    Write-Host "Error occurred: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Full error: $($_.Exception.ToString())" -ForegroundColor DarkRed
}