# Test Canadian Business Validation in Company Creation
# This script opens the company creation page and checks if Canadian validation is working

# Define the application URL
$appUrl = "https://localhost:44329"
$companyCreationUrl = "$appUrl/Companies/CreateCompany"

Write-Host "🍁 Testing Canadian Business Validation in PayrollPro" -ForegroundColor Green
Write-Host "=" * 60 -ForegroundColor Yellow

try {
    # Check if the application is running
    Write-Host "📡 Checking if PayrollPro is running at $appUrl..." -ForegroundColor Cyan
    
    $response = Invoke-WebRequest -Uri $appUrl -UseBasicParsing -TimeoutSec 30 -ErrorAction Stop
    
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Application is running successfully!" -ForegroundColor Green
        
        # Open the company creation page
        Write-Host "🏢 Opening company creation page..." -ForegroundColor Cyan
        Write-Host "URL: $companyCreationUrl" -ForegroundColor Yellow
        
        # Start the default browser with the company creation URL
        Start-Process $companyCreationUrl
        
        Write-Host ""
        Write-Host "🧪 Canadian Business Validation Test Checklist:" -ForegroundColor Green
        Write-Host "=" * 50 -ForegroundColor Yellow
        Write-Host "1. ✅ Open company creation form" -ForegroundColor Green
        Write-Host "2. 🔍 Check Canadian compliance notice is displayed" -ForegroundColor Yellow
        Write-Host "3. 📝 Test company name validation (try 'Maple Leaf Solutions Inc.')" -ForegroundColor Yellow
        Write-Host "4. 🏷️  Test company code formatting (try 'MAPLE-2024')" -ForegroundColor Yellow
        Write-Host "5. 📧 Test business email validation (avoid personal emails)" -ForegroundColor Yellow
        Write-Host "6. 📞 Test phone number formatting (try '4165551234')" -ForegroundColor Yellow
        Write-Host "7. 🏠 Test business address validation (try '123 Business St, Suite 456')" -ForegroundColor Yellow
        Write-Host "8. 🏙️  Test city validation (try 'Toronto')" -ForegroundColor Yellow
        Write-Host "9. 🗺️  Test province validation (try 'ON')" -ForegroundColor Yellow
        Write-Host "10. 📮 Test postal code formatting (try 'M5V3A8')" -ForegroundColor Yellow
        Write-Host "11. 🏛️  Test Business Number validation (try '123456789RT0001')" -ForegroundColor Yellow
        Write-Host "12. 📅 Test establishment date validation (try invalid dates)" -ForegroundColor Yellow
        Write-Host "13. 🌐 Test website validation (try 'https://example.ca')" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "💡 Expected Behaviors:" -ForegroundColor Cyan
        Write-Host "   • Real-time validation with visual feedback" -ForegroundColor White
        Write-Host "   • Auto-formatting for phone numbers and postal codes" -ForegroundColor White
        Write-Host "   • Canadian-specific placeholders and examples" -ForegroundColor White
        Write-Host "   • Business Number (BN) format validation" -ForegroundColor White
        Write-Host "   • Employment standards compliance checks" -ForegroundColor White
        Write-Host ""
        Write-Host "🔧 If validation doesn't work:" -ForegroundColor Yellow
        Write-Host "   • Press F12 to open browser dev tools" -ForegroundColor White
        Write-Host "   • Check Console for JavaScript errors" -ForegroundColor White
        Write-Host "   • Verify canadian-business-validation.js is loaded" -ForegroundColor White
        Write-Host "   • Test individual field validations" -ForegroundColor White
        
    } else {
        Write-Host "❌ Application returned status code: $($response.StatusCode)" -ForegroundColor Red
    }
    
} catch {
    Write-Host "❌ Error accessing the application: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "💡 Make sure the application is running with dotnet run" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "🎯 Test Results will be visible in the browser!" -ForegroundColor Green
Write-Host "📊 Check the form behavior matches Canadian business standards." -ForegroundColor Cyan