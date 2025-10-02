/**
 * PayrollPro Theme Switcher
 * Floating theme selector with modern UI
 */
class ThemeSwitcher {
    constructor() {
        this.themes = {
            'light': { name: 'Light', icon: 'fas fa-sun', color: '#fff3cd' },
            'dark': { name: 'Dark', icon: 'fas fa-moon', color: '#2d3748' },
            'system': { name: 'System', icon: 'fas fa-desktop', color: '#e2e8f0' },
            'auto': { name: 'Auto', icon: 'fas fa-magic', color: '#a78bfa' },
            'blue': { name: 'Blue', icon: 'fas fa-palette', color: '#3b82f6' },
            'green': { name: 'Green', icon: 'fas fa-leaf', color: '#10b981' }
        };
        
        this.currentTheme = this.getStoredTheme() || 'light';
        this.init();
    }

    init() {
        this.createThemeSwitcher();
        this.attachEventListeners();
        this.applyTheme(this.currentTheme);
    }

    createThemeSwitcher() {
        const container = document.createElement('div');
        container.className = 'payrollpro-theme-switcher';
        container.innerHTML = `
            <div class="theme-toggle-btn" title="Change Theme">
                <i class="fas fa-palette"></i>
            </div>
            <div class="theme-panel">
                <div class="theme-panel-header">
                    <i class="fas fa-palette"></i>
                    <span>Choose Theme</span>
                </div>
                <div class="theme-grid">
                    ${Object.entries(this.themes).map(([key, theme]) => `
                        <div class="theme-option" data-theme="${key}" title="${theme.name}">
                            <div class="theme-preview" style="background: ${theme.color}">
                                <i class="${theme.icon}"></i>
                            </div>
                            <span class="theme-name">${theme.name}</span>
                        </div>
                    `).join('')}
                </div>
            </div>
        `;
        
        // More aggressive approach - try multiple selectors and use fallback
        const possibleContainers = [
            '.lpx-navbar .lpx-navbar-nav',
            '.navbar .navbar-nav', 
            '.lpx-logo-container',
            '.lpx-navbar',
            '.navbar',
            'header',
            '.lpx-sidebar .lpx-logo-container'
        ];
        
        let targetContainer = null;
        for (const selector of possibleContainers) {
            targetContainer = document.querySelector(selector);
            if (targetContainer) {
                console.log('Theme switcher: Found container with selector:', selector);
                break;
            }
        }
        
        if (targetContainer) {
            // Check if it's a nav element that needs a nav-item wrapper
            if (targetContainer.classList.contains('lpx-navbar-nav') || 
                targetContainer.classList.contains('navbar-nav')) {
                const navItem = document.createElement('li');
                navItem.className = 'nav-item theme-switcher-nav';
                navItem.appendChild(container);
                targetContainer.appendChild(navItem);
            } else {
                // Direct append for other containers
                targetContainer.appendChild(container);
            }
        } else {
            // Fallback: Create as fixed positioned element
            console.log('Theme switcher: Using fallback positioning');
            container.style.position = 'fixed';
            container.style.top = '20px';
            container.style.right = '20px';
            container.style.zIndex = '9999';
            container.style.backgroundColor = 'rgba(22, 22, 22, 0.9)';
            container.style.padding = '8px';
            container.style.borderRadius = '8px';
            container.style.border = '1px solid rgba(145, 152, 165, 0.3)';
            document.body.appendChild(container);
        }
        
        this.container = container;
        console.log('Theme switcher created and added to:', targetContainer ? targetContainer.tagName + '.' + targetContainer.className : 'body (fallback)');
    }

    attachEventListeners() {
        const toggleBtn = this.container.querySelector('.theme-toggle-btn');
        const panel = this.container.querySelector('.theme-panel');
        const options = this.container.querySelectorAll('.theme-option');

        // Toggle panel visibility
        toggleBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            panel.classList.toggle('active');
        });

        // Close panel when clicking outside
        document.addEventListener('click', (e) => {
            if (!this.container.contains(e.target)) {
                panel.classList.remove('active');
            }
        });

        // Theme selection
        options.forEach(option => {
            option.addEventListener('click', () => {
                const theme = option.dataset.theme;
                this.setTheme(theme);
                panel.classList.remove('active');
            });
        });

        // ESC key to close
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && panel.classList.contains('active')) {
                panel.classList.remove('active');
            }
        });
    }

    setTheme(theme) {
        this.currentTheme = theme;
        this.applyTheme(theme);
        this.storeTheme(theme);
        this.updateActiveState();
    }

    applyTheme(theme) {
        document.documentElement.className = '';
        document.documentElement.classList.add(`theme-${theme}`);
        
        // Update meta theme-color for mobile browsers
        let metaThemeColor = document.querySelector('meta[name="theme-color"]');
        if (!metaThemeColor) {
            metaThemeColor = document.createElement('meta');
            metaThemeColor.name = 'theme-color';
            document.head.appendChild(metaThemeColor);
        }
        
        // Set theme color based on selected theme
        const themeColors = {
            'light': '#ffffff',
            'dark': '#1a202c',
            'system': '#f7fafc',
            'auto': '#805ad5',
            'blue': '#3182ce',
            'green': '#38a169'
        };
        
        metaThemeColor.content = themeColors[theme] || themeColors.light;
        
        // Apply CSS custom properties
        this.setCSSVariables(theme);
        
        // Trigger custom event for other components
        window.dispatchEvent(new CustomEvent('themeChanged', { 
            detail: { theme, themeName: this.themes[theme]?.name } 
        }));
    }

    setCSSVariables(theme) {
        const root = document.documentElement;
        
        const themeVars = {
            'light': {
                '--primary-bg': '#ffffff',
                '--secondary-bg': '#f8f9fa',
                '--text-primary': '#212529',
                '--text-secondary': '#6c757d',
                '--border-color': '#dee2e6',
                '--accent-color': '#0d6efd',
                '--card-shadow': '0 2px 4px rgba(0,0,0,0.1)'
            },
            'dark': {
                '--primary-bg': '#1a202c',
                '--secondary-bg': '#2d3748',
                '--text-primary': '#ffffff',
                '--text-secondary': '#a0aec0',
                '--border-color': '#4a5568',
                '--accent-color': '#63b3ed',
                '--card-shadow': '0 2px 4px rgba(0,0,0,0.3)'
            },
            'system': {
                '--primary-bg': '#f7fafc',
                '--secondary-bg': '#e2e8f0',
                '--text-primary': '#2d3748',
                '--text-secondary': '#718096',
                '--border-color': '#cbd5e0',
                '--accent-color': '#4299e1',
                '--card-shadow': '0 2px 4px rgba(0,0,0,0.1)'
            },
            'auto': {
                '--primary-bg': '#faf5ff',
                '--secondary-bg': '#e9d5ff',
                '--text-primary': '#553c9a',
                '--text-secondary': '#805ad5',
                '--border-color': '#d6bcfa',
                '--accent-color': '#8b5cf6',
                '--card-shadow': '0 2px 4px rgba(139,92,246,0.1)'
            },
            'blue': {
                '--primary-bg': '#f0f9ff',
                '--secondary-bg': '#bae6fd',
                '--text-primary': '#0c4a6e',
                '--text-secondary': '#0284c7',
                '--border-color': '#7dd3fc',
                '--accent-color': '#0ea5e9',
                '--card-shadow': '0 2px 4px rgba(14,165,233,0.1)'
            },
            'green': {
                '--primary-bg': '#f0fdf4',
                '--secondary-bg': '#bbf7d0',
                '--text-primary': '#14532d',
                '--text-secondary': '#16a34a',
                '--border-color': '#86efac',
                '--accent-color': '#22c55e',
                '--card-shadow': '0 2px 4px rgba(34,197,94,0.1)'
            }
        };

        const vars = themeVars[theme] || themeVars.light;
        Object.entries(vars).forEach(([property, value]) => {
            root.style.setProperty(property, value);
        });
    }

    updateActiveState() {
        const options = this.container.querySelectorAll('.theme-option');
        options.forEach(option => {
            option.classList.toggle('active', option.dataset.theme === this.currentTheme);
        });
    }

    getStoredTheme() {
        return localStorage.getItem('payrollpro-theme');
    }

    storeTheme(theme) {
        localStorage.setItem('payrollpro-theme', theme);
    }

    // Public API
    getCurrentTheme() {
        return this.currentTheme;
    }

    getAvailableThemes() {
        return this.themes;
    }

    // Handle system theme changes for 'system' mode
    initSystemThemeDetection() {
        if (window.matchMedia) {
            const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
            
            const handleSystemThemeChange = () => {
                if (this.currentTheme === 'system') {
                    this.applyTheme('system');
                }
            };

            mediaQuery.addEventListener('change', handleSystemThemeChange);
            
            // Initial check
            if (this.currentTheme === 'system') {
                handleSystemThemeChange();
            }
        }
    }
}

// Auto-initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('Theme switcher: DOM loaded, waiting for full page load...');
    
    // Wait for ABP and other scripts to fully load
    setTimeout(function() {
        try {
            console.log('Theme switcher: Initializing...');
            if (!window.payrollProThemeSwitcher) {
                window.payrollProThemeSwitcher = new ThemeSwitcher();
                window.payrollProThemeSwitcher.initSystemThemeDetection();
                console.log('Theme switcher: Successfully initialized');
            }
        } catch (error) {
            console.error('Theme switcher: Failed to initialize:', error);
        }
    }, 3000);
});

// Also try on window load as fallback
window.addEventListener('load', function() {
    setTimeout(function() {
        if (!window.payrollProThemeSwitcher) {
            console.log('Theme switcher: Fallback initialization...');
            try {
                window.payrollProThemeSwitcher = new ThemeSwitcher();
                window.payrollProThemeSwitcher.initSystemThemeDetection();
                console.log('Theme switcher: Fallback initialization successful');
            } catch (error) {
                console.error('Theme switcher: Fallback initialization failed:', error);
            }
        }
    }, 1000);
});

// Export for module systems
if (typeof module !== 'undefined' && module.exports) {
    module.exports = ThemeSwitcher;
}