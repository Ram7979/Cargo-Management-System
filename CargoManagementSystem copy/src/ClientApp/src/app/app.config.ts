import { ApplicationConfig, APP_INITIALIZER, LOCALE_ID, DEFAULT_CURRENCY_CODE } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

import { routes } from './app.routes';
import { registerLocaleData } from '@angular/common';
import localeIn from '@angular/common/locales/en-IN';
registerLocaleData(localeIn);
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { authInitializer } from './core/auth.initializer';
import { AuthService } from './core/services/auth.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), 
    provideAnimationsAsync(),
    // Use only functional interceptors — removes duplicate token injection
    provideHttpClient(
      withInterceptors([jwtInterceptor, errorInterceptor])
    ),
    {
      provide: APP_INITIALIZER,
      useFactory: authInitializer,
      deps: [AuthService],
      multi: true
    },
    { provide: LOCALE_ID, useValue: 'en-IN' },
    { provide: DEFAULT_CURRENCY_CODE, useValue: 'INR' }
  ]
};
