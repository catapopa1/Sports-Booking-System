import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { providePrimeNG } from 'primeng/config';
import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';
import { MessageService } from 'primeng/api';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';

// Pitchr — Emerald + Sky preset.
// Primary = Emerald scale, surfaces = Vanilla (light) / Carbon (dark).
const AppTheme = definePreset(Aura, {
  semantic: {
    primary: {
      50:  '#ECFDF5',
      100: '#D1FAE5',
      200: '#A7F3D0',
      300: '#6EE7B7',
      400: '#34D399',
      500: '#10B981',
      600: '#059669',
      700: '#047857',
      800: '#065F46',
      900: '#064E3B',
      950: '#022C22'
    },
    colorScheme: {
      light: {
        surface: {
          0:   '#FFFFFF',
          50:  '#FDFBF7',
          100: '#FCFAF4',
          200: '#F4F0E5',
          300: '#DDD4B8',
          400: '#C9C2A8',
          500: '#9AA0AB',
          600: '#4A5260',
          700: '#2E343F',
          800: '#232830',
          900: '#181C24',
          950: '#0E1115'
        }
      },
      dark: {
        surface: {
          0:   '#0E1115',
          50:  '#181C24',
          100: '#232830',
          200: '#2E343F',
          300: '#3D4351',
          400: '#545A68',
          500: '#9AA0AB',
          600: '#D1D5DC',
          700: '#E5E7EC',
          800: '#F2F4F7',
          900: '#FAFBFC',
          950: '#FFFFFF'
        }
      }
    }
  }
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
    MessageService,
    providePrimeNG({
      theme: {
        preset: AppTheme,
        options: { darkModeSelector: '.dark' }
      }
    })
  ]
};
