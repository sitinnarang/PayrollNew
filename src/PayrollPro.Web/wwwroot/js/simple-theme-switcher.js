/**
 * PayrollPro Simple Theme Switcher
 * Guaranteed visible theme selector
 */
class SimpleThemeSwitcher {
    constructor() {
        this.themes = {
            'light': { name: 'Light', icon: 'fas fa-sun' },
            'dark': { name: 'Dark', icon: 'fas fa-moon' },
            'system': { name: 'System', icon: 'fas fa-desktop' },
            'auto': { name: 'Auto', icon: 'fas fa-magic' },
            'blue': { name: 'Blue', icon: 'fas fa-palette' },
            'green': { name: 'Green', icon: 'fas fa-leaf' }
        };
        
        this.currentTheme = localStorage.getItem('payrollpro-theme') || 'light';
        this.init();
    }

    init() {
        console.log('SimpleThemeSwitcher: Initializing...');
        this.createSimpleSwitcher();
        this.applyTheme(this.currentTheme);
    }

    createSimpleSwitcher() {
        // Remove any existing switcher
        const existing = document.querySelector('#simple-theme-switcher');
        if (existing) {
            existing.remove();
        }

        // Create a simple, guaranteed visible theme switcher
        const container = document.createElement('div');
        container.id = 'simple-theme-switcher';
        container.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 999999;
            background: rgba(0, 0, 0, 0.8);
            border: 2px solid #007bff;
            border-radius: 8px;
            padding: 8px;
            color: white;
            font-family: Arial, sans-serif;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
        `;

        // Create toggle button
        const toggleBtn = document.createElement('div');
        toggleBtn.style.cssText = `
            cursor: pointer;
            padding: 8px 12px;
            border-radius: 4px;
            background: #007bff;
            text-align: center;
            font-size: 16px;
            transition: all 0.3s ease;
        `;
        toggleBtn.innerHTML = '<i class="fas fa-palette"></i>';
        toggleBtn.title = 'Theme Switcher';

        // Create theme panel
        const panel = document.createElement('div');
        panel.style.cssText = `
            position: absolute;
            top: 100%;
            right: 0;
            margin-top: 8px;
            background: rgba(0, 0, 0, 0.9);
            border: 1px solid #007bff;
            border-radius: 6px;
            padding: 8px;
            display: none;
            min-width: 150px;
        `;

        // Add theme options
        Object.entries(this.themes).forEach(([key, theme]) => {
            const option = document.createElement('div');
            option.style.cssText = `
                padding: 8px 12px;
                cursor: pointer;
                border-radius: 4px;
                margin: 2px 0;
                display: flex;
                align-items: center;
                gap: 8px;
                transition: background 0.3s ease;
                ${key === this.currentTheme ? 'background: #007bff;' : ''}
            `;
            option.innerHTML = `<i class="${theme.icon}"></i> ${theme.name}`;
            
            option.addEventListener('mouseenter', () => {
                if (key !== this.currentTheme) {
                    option.style.background = 'rgba(0, 123, 255, 0.3)';
                }
            });
            
            option.addEventListener('mouseleave', () => {
                if (key !== this.currentTheme) {
                    option.style.background = 'transparent';
                }
            });
            
            option.addEventListener('click', () => {
                this.switchTheme(key);
                panel.style.display = 'none';
                
                // Update active state
                panel.querySelectorAll('div').forEach(opt => opt.style.background = 'transparent');
                option.style.background = '#007bff';
            });
            
            panel.appendChild(option);
        });

        // Toggle panel visibility
        toggleBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            panel.style.display = panel.style.display === 'none' ? 'block' : 'none';
        });

        // Close panel when clicking outside
        document.addEventListener('click', () => {
            panel.style.display = 'none';
        });

        container.appendChild(toggleBtn);
        container.appendChild(panel);
        document.body.appendChild(container);
        
        console.log('SimpleThemeSwitcher: Created and added to page');
    }

    switchTheme(theme) {
        this.currentTheme = theme;
        localStorage.setItem('payrollpro-theme', theme);
        this.applyTheme(theme);
        console.log('SimpleThemeSwitcher: Switched to theme:', theme);
    }

    applyTheme(theme) {
        const root = document.documentElement;
        
        // Remove existing theme classes
        root.classList.remove('theme-light', 'theme-dark', 'theme-system', 'theme-auto', 'theme-blue', 'theme-green');
        
        // Add new theme class
        root.classList.add(`theme-${theme}`);
        
        // Apply theme-specific CSS variables
        const themeVariables = {
            'light': {
                '--primary-bg': '#ffffff',
                '--secondary-bg': '#f8f9fa',
                '--text-primary': '#333333',
                '--text-secondary': '#666666',
                '--border-color': '#dee2e6',
                '--accent-color': '#007bff'
            },
            'dark': {
                '--primary-bg': '#1a1a1a',
                '--secondary-bg': '#2d2d2d',
                '--text-primary': '#ffffff',
                '--text-secondary': '#cccccc',
                '--border-color': '#404040',
                '--accent-color': '#4dabf7'
            },
            'blue': {
                '--primary-bg': '#e3f2fd',
                '--secondary-bg': '#bbdefb',
                '--text-primary': '#0d47a1',
                '--text-secondary': '#1565c0',
                '--border-color': '#90caf9',
                '--accent-color': '#2196f3'
            },
            'green': {
                '--primary-bg': '#e8f5e8',
                '--secondary-bg': '#c8e6c9',
                '--text-primary': '#1b5e20',
                '--text-secondary': '#2e7d32',
                '--border-color': '#81c784',
                '--accent-color': '#4caf50'
            }
        };
        
        const variables = themeVariables[theme] || themeVariables['light'];
        Object.entries(variables).forEach(([property, value]) => {
            root.style.setProperty(property, value);
        });
        
        console.log('SimpleThemeSwitcher: Applied theme variables for:', theme);
    }
}

// Initialize immediately if DOM is ready, otherwise wait
console.log('Simple theme switcher script loaded');

function initSimpleThemeSwitcher() {
    try {
        new SimpleThemeSwitcher();
    } catch (error) {
        console.error('Error initializing SimpleThemeSwitcher:', error);
    }
}

if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSimpleThemeSwitcher);
} else {
    initSimpleThemeSwitcher();
}

// Backup initialization after delay
setTimeout(() => {
    if (!document.querySelector('#simple-theme-switcher')) {
        console.log('Simple theme switcher not found, creating backup...');
        initSimpleThemeSwitcher();
    }
}, 2000);