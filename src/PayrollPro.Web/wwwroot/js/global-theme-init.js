// Global Theme Switcher Initialization for ABP Framework
(function() {
    'use strict';

    // Wait for ABP and theme resources to load
    function initializeGlobalThemeSwitcher() {
        // Check if theme switcher is already loaded
        if (window.payrollProTheme) {
            return;
        }

        // Create script element to load theme switcher
        const script = document.createElement('script');
        script.src = '/js/theme-switcher.js';
        script.onload = function() {
            console.log('PayrollPro Theme Switcher loaded successfully');
        };
        script.onerror = function() {
            console.error('Failed to load PayrollPro Theme Switcher');
        };
        
        document.head.appendChild(script);
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initializeGlobalThemeSwitcher);
    } else {
        initializeGlobalThemeSwitcher();
    }

    // Also initialize after ABP framework loads
    if (window.abp) {
        initializeGlobalThemeSwitcher();
    } else {
        // Wait for ABP to load
        const checkABP = setInterval(() => {
            if (window.abp) {
                clearInterval(checkABP);
                initializeGlobalThemeSwitcher();
            }
        }, 100);
    }

})();