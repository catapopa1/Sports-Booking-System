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

const AppTheme = definePreset(Aura, {
  semantic: {
    primary: {
      50:  '#eef2ff',
      100: '#dce6ff',
      200: '#b9ccff',
      300: '#85a7fc',
      400: '#5480f9',
      500: '#275BF5',
      600: '#1a47d4',
      700: '#1436ab',
      800: '#112a87',
      900: '#0f2268',
      950: '#091444'
    },
    colorScheme: {
      light: {
        surface: {
          0:   '#ffffff',
          50:  '#f7f7f3',
          100: '#efefe9',
          200: '#e2e2da',
          300: '#d1d1c7',
          400: '#a8a89a',
          500: '#7e7e72',
          600: '#5c5c52',
          700: '#3d3d35',
          800: '#252520',
          900: '#131310',
          950: '#090907'
        }
      },
      dark: {
        surface: {
          0:   '#0a1628',
          50:  '#0f1f38',
          100: '#152847',
          200: '#1c3358',
          300: '#243f6b',
          400: '#2f507f',
          500: '#3d6496',
          600: '#5a7fb0',
          700: '#7d9dc5',
          800: '#a4bcd8',
          900: '#ccdaeb',
          950: '#e8eef6'
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
