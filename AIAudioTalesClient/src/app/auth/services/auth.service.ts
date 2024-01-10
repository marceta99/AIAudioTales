import { Injectable, NgZone } from '@angular/core';
import { environment } from 'src/environment/environment';
import { HttpClient, HttpHeaderResponse, HttpHeaders } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { RegisterUser, User } from 'src/app/entities';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private path = environment.apiUrl;
  loggedUser!: User | null;
  isLoggedIn: boolean = false;

  constructor(private httpClient: HttpClient, private _ngZone: NgZone, private router: Router) { }

  public login(email: string, password: string): Observable<User>{
    const user = {
      "email": email,
      "password": password
    };
    return this.httpClient.post<User>(
      this.path + "Auth/Login", user, {withCredentials: true}
    ).pipe(tap((response : User) =>{
        console.log(response)
        this.loggedUser = response;
        this.isLoggedIn = true;
        //localStorage.setItem("c23fj2",JSON.stringify(this.customEncode(response)));

        this._ngZone.run(() => {
          this.router.navigate(['/home']);
        })
    }))
  }
  public register(user: RegisterUser):Observable<any>{
    return this.httpClient.post(this.path+"Auth/Register", user );
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
    ).pipe(tap((response : User) =>{
      console.log(response)
      this.loggedUser = response;
      this.isLoggedIn = true;
      //localStorage.setItem("c23fj2",JSON.stringify(this.customEncode(response)));

      this._ngZone.run(() => {
        this.router.navigate(['/home']);
      })
  }));
  }
  public refreshToken(): Observable<any> {
    return this.httpClient.get(this.path + "Auth/RefreshToken");
  }
  public getCurrentUser(): Observable<User> {
    return this.httpClient.get<User>(this.path + "Auth/GetCurrentUser");
  }

  private customEncode(data: User) {
    let encoded = '';
    const str = JSON.stringify(data);
    for (let i = str.length - 1; i >= 0; i--) {
        encoded += str[i] + String.fromCharCode(65 + Math.floor(Math.random() * 26));
    }
    return encoded;
  }
  private customDecode(encoded : string) {
      let decoded = '';
      for (let i = 0; i < encoded.length; i += 2) {
          decoded = encoded[i] + decoded;
      }
      return JSON.parse(decoded);
  }

}
