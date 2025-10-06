# PowerShell script to grant Admin user employee creation permissions

Write-Host "=== PayrollPro Admin Permission Fix ===" -ForegroundColor Green
Write-Host "Problem: Current admin user (fd3d5e47-dbeb-4bdc-b35e-1aec1e54109f) lacks PayrollPro.Employees.Create permission" -ForegroundColor Yellow

# Current user ID from logs
$adminUserId = "fd3d5e47-dbeb-4bdc-b35e-1aec1e54109f"
$baseUrl = "https://localhost:44329"

Write-Host "Admin User ID: $adminUserId" -ForegroundColor Cyan
Write-Host "Base URL: $baseUrl" -ForegroundColor Cyan

Write-Host "`nTo fix this issue manually:" -ForegroundColor Yellow
Write-Host "1. Go to: $baseUrl/Identity/Users" -ForegroundColor White
Write-Host "2. Find the admin user" -ForegroundColor White
Write-Host "3. Click 'Actions' > 'Permissions'" -ForegroundColor White
Write-Host "4. Enable 'PayrollPro.Employees.Create' permission" -ForegroundColor White
Write-Host "5. Save changes" -ForegroundColor White

Write-Host "`nAlternatively, add the admin user to company-user role:" -ForegroundColor Yellow
Write-Host "1. Go to: $baseUrl/Identity/Users" -ForegroundColor White
Write-Host "2. Find the admin user" -ForegroundColor White
Write-Host "3. Click 'Actions' > 'Roles'" -ForegroundColor White
Write-Host "4. Assign 'company-user' role" -ForegroundColor White
Write-Host "5. Save changes" -ForegroundColor White

Write-Host "`nOpening User Management..." -ForegroundColor Green
Start-Process "$baseUrl/Identity/Users"

Write-Host "`nScript completed. Please follow the manual steps above to grant permissions." -ForegroundColor Green