import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthStorageService } from './auth-storage.service';
import { environment } from 'src/environments/environment';
import { BehaviorSubject, Observable, from, of } from 'rxjs';
import { switchMap, map } from 'rxjs/operators';
import { isNativeApp } from 'src/utils/functions';
import { ApiMessageResponse, RegisterCreator, RegisterUser, User } from 'src/app/entities';

interface MobileLoginResponse {
  accessToken: string;
  refreshToken: string;
  user: User
}
interface WebLoginResponse {
  user: User
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

  private currentUserSubject: BehaviorSubject<User | undefined> = new BehaviorSubject<User | undefined>(undefined);
  public currentUser$: Observable<User | undefined> = this.currentUserSubject.asObservable();

  public get currentUser(): User | undefined {
    return this.currentUserSubject.value;
  }

  public set currentUser(user: User) {
    this.currentUserSubject.next(user);
  }

  constructor(
    private http: HttpClient,
    private storageService: AuthStorageService
  ) {}

  public login(email: string, password: string): Observable<User> {
    if (isNativeApp()) {
      // mobile => call /login-mobile
      return this.http.post<MobileLoginResponse>(
        `${this.baseUrl}/login-mobile`,
        { email, password }
      ).pipe(
        switchMap(res => {
          this.currentUser = res.user;
          // store tokens
          return from(this.storageService.setTokens(res.accessToken, res.refreshToken)).pipe(
            map(() => res.user)
          );
        })
      );
    } else {
      // web => call /login-web (withCredentials so cookies are set)
      return this.http.post<WebLoginResponse>(
        `${this.baseUrl}/login-web`,
        { email, password },
        { withCredentials: true}
      ).pipe(map(response => {
        this.currentUser = response.user;
        return response.user;
      }));
    }
  }

  public refresh(): Observable<MobileRefreshResponse | ApiMessageResponse | string> {
    if (isNativeApp()) {
      // mobile => call /refresh-mobile with the stored refresh token
      return from(this.storageService.getRefreshToken()).pipe(
        switchMap(rToken => {
          if (!rToken) {
            return of('No refresh token found');
          }
          return this.http.post<MobileRefreshResponse>(
            `${this.baseUrl}/refresh-mobile`,
            { refreshToken: rToken },
            { withCredentials: true}
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
      return this.http.post<ApiMessageResponse>(
        `${this.baseUrl}/refresh-web`,
        {},
        { withCredentials: true }
      );
    }
  }

  public logout(): Observable<ApiMessageResponse | string> {
    if (isNativeApp()) {
      // mobile => call /logout-mobile + remove tokens from storage
      return from(this.storageService.getRefreshToken()).pipe(
        switchMap(rToken => {
          if (!rToken) {
            // no token, just clear anyway
            return from(this.storageService.clearTokens()).pipe(map(() => 'No refresh token found.'));
          }
          return this.http.post<ApiMessageResponse>(`${this.baseUrl}/logout-mobile`, { refreshToken: rToken })
            .pipe(
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
      return this.http.post<ApiMessageResponse>(
        `${this.baseUrl}/logout-web`,
        {},
        { withCredentials: true}
      );
    }
  }

  public isLoggedIn(): Observable<boolean> {
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

  public register(user: RegisterUser):Observable<ApiMessageResponse>{
    return this.http.post<ApiMessageResponse>(`${this.baseUrl}/register`, user);
  }

  public registerCreator(creator: RegisterCreator): Observable<ApiMessageResponse>{
    return this.http.post<ApiMessageResponse>(`${this.baseUrl}/register-creator`, creator);
  }

  public getCurrentUser(): void {
     this.http
         .get<User>(`${this.baseUrl}/current-user`, { withCredentials: true})
         .subscribe(user => this.currentUser = user)
  }
}
