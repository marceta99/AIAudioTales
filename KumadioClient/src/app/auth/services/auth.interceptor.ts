import { inject } from '@angular/core';
import {
  HttpContextToken,
  HttpRequest,
  HttpEvent,
  HttpHandlerFn,
  HttpErrorResponse,
  HttpHeaders
} from '@angular/common/http';
import { Observable, throwError, from, of } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Capacitor } from '@capacitor/core';
import { AuthService } from './auth.service';
import { AuthStorageService } from './auth-storage.service';

// The token that indicates whether we've tried to refresh
export const REFRESH_ATTEMPTED = new HttpContextToken<boolean>(() => false);

export function authInterceptor(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> {

  // 2) We can inject the Angular services in function-based interceptors
  const router = inject(Router);
  const authService = inject(AuthService);
  const storageService = inject(AuthStorageService);

  // Check if request uses formData
  const isFormDataRequest =
    req.url.includes('AddRootPart') || req.url.includes('AddBookPart');

  // Decide between mobile vs. web
  if (Capacitor.isNativePlatform()) {
    // Mobile → attach Bearer token
    if (!isFormDataRequest) {
      const headers = new HttpHeaders().set('Content-Type', 'application/json');
      req = req.clone({ headers });
    }
    // retrieve token from secure storage
    return from(storageService.getAccessToken()).pipe(
      switchMap(token => {
        if (token) {
          req = req.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });
        }
        // pass request to next, handle 401
        return next(req).pipe(
          catchError(err => handleAuthError(err, req, next, router, authService))
        );
      })
    );
  } else {
    // Web → attach withCredentials: true, rely on cookies
    if (!isFormDataRequest) {
      const headers = new HttpHeaders().set('Content-Type', 'application/json');
      req = req.clone({
        headers,
        withCredentials: true
      });
    } else {
      req = req.clone({ withCredentials: true });
    }
    return next(req).pipe(
      catchError(err => handleAuthError(err, req, next, router, authService))
    );
  }
}

function handleAuthError(
  err: HttpErrorResponse,
  originalReq: HttpRequest<unknown>,
  next: HttpHandlerFn,
  router: Router,
  authService: AuthService
): Observable<HttpEvent<unknown>> {
  if (err?.status === 401) {
    // check if we've already tried refreshing on this request
    const alreadyTried = originalReq.context.get(REFRESH_ATTEMPTED);
    if (!alreadyTried) {
      // we haven't tried yet → attempt refresh
      return authService.refresh().pipe(
        switchMap(() => {
          // set REFRESH_ATTEMPTED = true in the request context
          // so we don't keep looping if refresh fails again
          const newContext = originalReq.context.set(REFRESH_ATTEMPTED, true);

          // clone the original request with the updated context
          const retriedReq = originalReq.clone({
            context: newContext
          });

          // re-issue the request
          return next(retriedReq).pipe(
            catchError(retryErr => {
              console.error('Retried request failed:', retryErr);
              return throwError(() => retryErr);
            })
          );
        }),
        catchError(refreshErr => {
          console.warn('Refresh token failed → forcing logout.', refreshErr);
          authService.logout().subscribe({
            next: () => {
              router.navigate(['/login']).then(() => window.location.reload());
            }
          });
          return throwError(() => refreshErr);
        })
      );
    }
  }
  // if we get here, it's either not a 401 or we've already tried refresh
  return throwError(() => err);
}
