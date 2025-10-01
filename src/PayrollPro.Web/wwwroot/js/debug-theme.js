// Simple debug script to test theme switcher visibility
console.log('Debug theme script loaded');

function debugThemeSwitcher() {
    console.log('=== DEBUGGING THEME SWITCHER ===');
    
    // Check if DOM is ready
    console.log('DOM ready state:', document.readyState);
    
    // Log all navigation-related elements
    const allNavElements = document.querySelectorAll('*');
    const navRelatedElements = [];
    
    allNavElements.forEach(el => {
        if (el.className && (
            el.className.includes('nav') || 
            el.className.includes('header') ||
            el.className.includes('lpx') ||
            el.className.includes('toolbar')
        )) {
            navRelatedElements.push({
                tagName: el.tagName,
                className: el.className,
                id: el.id
            });
        }
    });
    
    console.log('All navigation-related elements found:', navRelatedElements);
    
    // Check existing elements
    const navSelectors = [
        '.lpx-navbar',
        '.lpx-navbar .lpx-navbar-nav',
        '.navbar',
        '.navbar .navbar-nav',
        '.lpx-logo-container',
        'header',
        '.lpx-sidebar',
        '[class*="nav"]',
        '[class*="toolbar"]',
        '[class*="header"]'
    ];
    
    console.log('Testing specific selectors:');
    navSelectors.forEach(selector => {
        const element = document.querySelector(selector);
        if (element) {
            console.log('✓ Found:', selector, 'Classes:', element.className, 'Element:', element);
        } else {
            console.log('✗ Not found:', selector);
        }
    });
    
    // Check if theme switcher already exists
    const existingSwitcher = document.querySelector('.payrollpro-theme-switcher');
    console.log('Existing theme switcher:', existingSwitcher);
    
    // Create a simple visible test element
    const testDiv = document.createElement('div');
    testDiv.id = 'theme-debug-test';
    testDiv.style.cssText = `
        position: fixed;
        top: 10px;
        right: 10px;
        background: red;
        color: white;
        padding: 10px;
        z-index: 99999;
        border: 2px solid yellow;
        font-weight: bold;
    `;
    testDiv.textContent = 'THEME DEBUG - Can you see this?';
    document.body.appendChild(testDiv);
    
    console.log('Test element created:', testDiv);
    
    // Try to create a theme switcher manually
    const manualSwitcher = document.createElement('div');
    manualSwitcher.id = 'manual-theme-test';
    manualSwitcher.style.cssText = `
        position: fixed;
        top: 60px;
        right: 10px;
        background: blue;
        color: white;
        padding: 15px;
        z-index: 99999;
        border: 2px solid green;
        cursor: pointer;
    `;
    manualSwitcher.innerHTML = '<i class="fas fa-palette"></i> MANUAL THEME';
    document.body.appendChild(manualSwitcher);
    
    console.log('Manual theme switcher created:', manualSwitcher);
}

// Run debug immediately and after DOM loads
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', debugThemeSwitcher);
} else {
    debugThemeSwitcher();
}

// Also run after a delay to catch late-loading elements
setTimeout(debugThemeSwitcher, 3000);