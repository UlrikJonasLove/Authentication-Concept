import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withEnabledBlockingInitialNavigation } from '@angular/router';
import { environment } from '../environments/environment';
import { API_BASE_URL } from './app.tokens';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { AuthService } from './core/services/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideRouter(routes, withEnabledBlockingInitialNavigation()),
    provideAppInitializer(() => inject(AuthService).restoreSession()),
    {
      provide: API_BASE_URL,
      useValue: environment.apiBaseUrl,
    },
  ],
};
