import { bootstrapApplication } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { importProvidersFrom } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideRouter, withPreloading, PreloadAllModules } from '@angular/router';
import { provideIonicAngular, IonicRouteStrategy } from '@ionic/angular/standalone';
import { RouteReuseStrategy } from '@angular/router';

import { AppComponent } from './app/app.component';
import { authInterceptor} from './app/auth/services/auth.interceptor';
import { routes } from './app/app.routes';

bootstrapApplication(AppComponent, {
  providers: [
    // For Ionic
    { provide: RouteReuseStrategy, useClass: IonicRouteStrategy },
    provideIonicAngular(),

    // Routing
    provideRouter(routes, withPreloading(PreloadAllModules)),

    // Provide HttpClient with the AuthInterceptor
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),

    // Provide the rest
    importProvidersFrom(BrowserModule),
    importProvidersFrom(BrowserAnimationsModule)
  ],
});
