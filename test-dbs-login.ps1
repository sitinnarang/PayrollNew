# Test DBS user login
param (
    [string]$baseUrl = "https://localhost:44329"
)

# Skip SSL certificate validation
[System.Net.ServicePointManager]::ServerCertificateValidationCallback = {$true}

Write-Host "Testing DBS user login..."

# Step 1: Get the login page to retrieve the anti-forgery token
try {
    $loginPageResponse = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -SessionVariable websession -UseBasicParsing
    Write-Host "✓ Login page accessed successfully"
    
    # Extract the anti-forgery token from the form
    $antiForgeryToken = ""
    if ($loginPageResponse.Content -match '__RequestVerificationToken.*?value="([^"]+)"') {
        $antiForgeryToken = $matches[1]
        Write-Host "✓ Anti-forgery token extracted: $($antiForgeryToken.Substring(0,20))..."
    }
    
    # Step 2: Perform the login
    $loginData = @{
        UserName = "DBS"
        Password = "DBSPass123!"
        RememberMe = "false"
        __RequestVerificationToken = $antiForgeryToken
    }
    
    $loginResponse = Invoke-WebRequest -Uri "$baseUrl/Account/Login" -Method Post -Body $loginData -WebSession $websession -UseBasicParsing
    
    Write-Host "Login response status: $($loginResponse.StatusCode)"
    Write-Host "Login response headers:"
    $loginResponse.Headers | ForEach-Object { Write-Host "  $($_.Key): $($_.Value)" }
    
    # Check if redirected (successful login usually redirects)
    if ($loginResponse.StatusCode -eq 302) {
        $location = $loginResponse.Headers.Location
        Write-Host "✓ Login successful - redirected to: $location"
        
        # Follow the redirect
        if ($location) {
            $redirectResponse = Invoke-WebRequest -Uri $location -WebSession $websession -UseBasicParsing
            Write-Host "Redirect response status: $($redirectResponse.StatusCode)"
            
            # Check if it contains company-specific content
            if ($redirectResponse.Content -match "DBS Bank" -or $redirectResponse.Content -match "Company Management" -or $redirectResponse.Content -match "My Company") {
                Write-Host "✓ SUCCESS: Company user interface detected!"
            } else {
                Write-Host "⚠ Login successful but no company-specific UI detected"
            }
        }
    } else {
        Write-Host "✗ Login may have failed - status: $($loginResponse.StatusCode)"
        Write-Host "Response content preview:"
        Write-Host $loginResponse.Content.Substring(0, [Math]::Min(500, $loginResponse.Content.Length))
    }
    
} catch {
    Write-Host "✗ Error during testing: $($_.Exception.Message)"
    if ($_.Exception.Response) {
        Write-Host "Response status: $($_.Exception.Response.StatusCode)"
        Write-Host "Response headers: $($_.Exception.Response.Headers)"
    }
}

Write-Host "`nTest completed."