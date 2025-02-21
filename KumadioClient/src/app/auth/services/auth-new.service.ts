// src/app/core/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthStorageService } from './auth-storage.service';
import { environment } from 'src/environments/environment';
import { Observable, from, of } from 'rxjs';
import { switchMap, map } from 'rxjs/operators';
import { isNativeApp } from 'src/utils/functions';
import { RegisterUser } from 'src/app/entities';

interface MobileLoginResponse {
  accessToken: string;
  refreshToken: string;
}

interface MobileRefreshResponse {
  accessToken: string;
  refreshToken: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private baseUrl = environment.apiUrl + '/auth';

  constructor(
    private http: HttpClient,
    private storageService: AuthStorageService
  ) {}

  login(email: string, password: string): Observable<any> {
    if (isNativeApp()) {
      // mobile => call /login-mobile
      return this.http.post<MobileLoginResponse>(
        `${this.baseUrl}/login-mobile`,
        { email, password }
      ).pipe(
        switchMap(res => {
          // store tokens
          return from(this.storageService.setTokens(res.accessToken, res.refreshToken)).pipe(
            map(() => res)
          );
        })
      );
    } else {
      // web => call /login-web (withCredentials so cookies are set)
      return this.http.post(
        `${this.baseUrl}/login-web`,
        { email, password },
        { withCredentials: true, responseType: 'text' }
      );
    }
  }

  refresh(): Observable<any> {
    if (isNativeApp()) {
      // mobile => call /refresh-mobile with the stored refresh token
      return from(this.storageService.getRefreshToken()).pipe(
        switchMap(rToken => {
          if (!rToken) {
            return of({ error: 'No refresh token found' });
          }
          return this.http.post<MobileRefreshResponse>(
            `${this.baseUrl}/refresh-mobile`,
            { refreshToken: rToken }
          ).pipe(
            switchMap(res => {
              // store new tokens
              return from(this.storageService.setTokens(res.accessToken, res.refreshToken)).pipe(
                map(() => res)
              );
            })
          );
        })
      );
    } else {
      // web => call /refresh-web (withCredentials)
      return this.http.post(
        `${this.baseUrl}/refresh-web`,
        {},
        { withCredentials: true, responseType: 'text' }
      );
    }
  }

  logout(): Observable<any> {
    if (isNativeApp()) {
      // mobile => call /logout-mobile + remove tokens from storage
      return from(this.storageService.getRefreshToken()).pipe(
        switchMap(rToken => {
          if (!rToken) {
            // no token, just clear anyway
            return from(this.storageService.clearTokens()).pipe(map(() => 'No refresh token found.'));
          }
          return this.http.post(
            `${this.baseUrl}/logout-mobile`,
            { refreshToken: rToken },
            { responseType: 'text' }
          ).pipe(
            switchMap(res => 
              from(this.storageService.clearTokens()).pipe(
                map(() => res)
              )
            )
          );
        })
      );
    } else {
      // web => call /logout-web, cookies are cleared
      return this.http.post(
        `${this.baseUrl}/logout-web`,
        {},
        { withCredentials: true, responseType: 'text' }
      );
    }
  }

  isLoggedIn(): Observable<boolean> {
    if (isNativeApp()) {
      return from(this.storageService.getAccessToken()).pipe(
        map(token => !!token)
      );
    } else {
      // web might do a call to a user endpoint or something
      // for now, let's just return an observable of true
      return of(true);
    }
  }

  public register(user: RegisterUser):Observable<any>{
    return this.http.post(
      `${this.baseUrl}/register`,
      user,
      { responseType: 'text' });
    }
}
