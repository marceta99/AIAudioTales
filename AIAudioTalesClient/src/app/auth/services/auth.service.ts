import { Injectable, NgZone } from '@angular/core';
import { environment } from 'src/environment/environment';
import { HttpClient, HttpHeaderResponse, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { RegisterCreator, RegisterUser, Role, User } from 'src/app/entities';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private path = environment.apiUrl;
  currentUser: BehaviorSubject<User | null> = new BehaviorSubject<User | null>(null);
  isLoggedIn: boolean = false;

  constructor(private httpClient: HttpClient, private _ngZone: NgZone, private router: Router) { }

  public login(email: string, password: string): Observable<User>{
    const user = {
      "email": email,
      "password": password
    };
    return this.httpClient.post<User>(
      this.path + "Auth/Login", user, {withCredentials: true}
    ).pipe(tap((user : User) =>{
        console.log(user)
        this.currentUser.next(user);
        this.isLoggedIn = true;
        //localStorage.setItem("c23fj2",JSON.stringify(this.customEncode(response)));

        this._ngZone.run(() => {
          if(user.role === Role.CREATOR){
            this.router.navigate(['/home/statistics']);
          }else{
            this.router.navigate(['/home']);
          }
        })
    }))
  }

  public register(user: RegisterUser):Observable<any>{
    return this.httpClient.post(this.path+"Auth/Register", user );
  }

  public registerCreator(creator: RegisterCreator):Observable<any>{
    return this.httpClient.post(this.path+"Auth/RegisterCreator", creator );
  }

  public loginWithGoogle(credentials: string): Observable<User>{
    const headers = new HttpHeaders().set('Content-type', 'application/json');

    const requestOptions = {
      headers: headers,
      withCredentials: true
    }
    return this.httpClient.post<User>(
      this.path + "Auth/LoginWithGoogle",
      JSON.stringify(credentials),
      requestOptions
    ).pipe(tap((user : User) =>{
      console.log(user)
      this.currentUser.next(user);
      this.isLoggedIn = true;
      //localStorage.setItem("c23fj2",JSON.stringify(this.customEncode(response)));

      this._ngZone.run(() => {
        if(user.role === Role.CREATOR){
          this.router.navigate(['/home/statistics']);
        }else{
          this.router.navigate(['/home']);
        }
      })
  }));
  }

  public refreshToken(): Observable<any> {
    return this.httpClient.get(this.path + "Auth/RefreshToken");
  }

  public getCurrentUser(): Observable<User> {
    return this.httpClient.get<User>(this.path + "Auth/GetCurrentUser");
  }
}
