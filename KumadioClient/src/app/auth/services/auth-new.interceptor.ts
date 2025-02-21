import { inject, Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
  HttpHeaders,
  HttpHandlerFn
} from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, throwError, from, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { Capacitor } from '@capacitor/core';
import { AuthStorageService } from './auth-storage.service';  
import { AuthService } from './auth-new.service';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptor implements HttpInterceptor {
  private errorCounter = 0;

  constructor(
    private router: Router,
    private authService: AuthService,
    private storageService: AuthStorageService
  ) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    // Check if request uses formData
    const isFormDataRequest =
      request.url.includes('AddRootPart') ||
      request.url.includes('AddBookPart');

    // If we’re on web, rely on cookies. If on mobile, attach Bearer token
    if (Capacitor.isNativePlatform()) {
      // Mobile scenario attach bearer token inside header
      if (!isFormDataRequest) {
        const headers = new HttpHeaders().set('Content-type', 'application/json');
        request = request.clone({ headers });
      }

      // Attach Bearer token from storage
      return from(this.storageService.getAccessToken()).pipe(
        switchMap(token => {
          if (token) {
            request = request.clone({
              setHeaders: {
                Authorization: `Bearer ${token}`
              }
            });
          }

          return next.handle(request).pipe(
            catchError(err => this.handleAuthError(err, request, next))
          );
        })
      );

    } else {
      // Web scenario: use cookies with credentials
      if (!isFormDataRequest) {
        const headers = new HttpHeaders().set('Content-type', 'application/json');
        request = request.clone({
          headers,
          withCredentials: true
        });
      } else {
        // For formData uploads, just set withCredentials = true
        request = request.clone({
          withCredentials: true
        });
      }

      return next.handle(request).pipe(
        catchError(err => this.handleAuthError(err, request, next))
      );
    }
  }

  private handleAuthError(
    err: HttpErrorResponse,
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<any> {
    // If 401, try refresh once
    if (err && err.status === 401 && this.errorCounter < 1) {
      this.errorCounter++;

      // attempt refresh
      return this.authService.refresh().pipe(
        switchMap(() => {
          // reset counter
          this.errorCounter = 0;
          // retry original request
          return next.handle(request).pipe(
            catchError((retryErr: any) => {
              console.error('Retried request failed:', retryErr);
              return throwError(() => new Error(retryErr));
            })
          );
        }),
        catchError((refreshErr: any) => {
          // Refresh failed => force logout
          console.warn('Refresh token failed. Logging out.', refreshErr);
          this.errorCounter = 0;
          this.authService.logout().subscribe({
            next: () => {
              this.router.navigate(['/login']).then(() => window.location.reload());
            }
          });
          return throwError(() => new Error(refreshErr));
        })
      );
    } else {
      // Non-401 or second-time failure => pass it through
      this.errorCounter = 0;
      return throwError(() => err);
    }
  }
}
