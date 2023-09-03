import { of, throwError } from 'rxjs';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, catchError, switchMap } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  errorCounter = 0;

  constructor(private router: Router, private authService: AuthService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    request = request.clone({
      withCredentials: true
    });
    return next.handle(request).pipe(catchError(err => this.handleAuthError(err, request, next)));
  }

  private handleAuthError(err: HttpErrorResponse, request: HttpRequest<any>, next: HttpHandler): Observable<any> {
    if (err && err.status === 401 && this.errorCounter !== 1) {
      this.errorCounter++;
      return this.authService.refreshToken().pipe(
        switchMap(() => {
          alert('token refreshed');

          // Re-send the original request after token refresh
          return next.handle(request).pipe(
            catchError((retryErr: any) => {
              // Handle the error from retrying the original request
              console.error('Retried request failed:', retryErr);
              return throwError(()=> new Error(retryErr));
            })
          );
        }),
        catchError((refreshErr: any) => {
          return this.authService.revokeToken().pipe(
            switchMap(() => {
              this.router.navigate(['/login']).then(()=> window.location.reload());
              return throwError(()=> new Error(refreshErr));
            })
          );
        })
      );
    } else {
      this.errorCounter = 0;
      return throwError(() => new Error('Non Authentication error'));
    }
  }
}
