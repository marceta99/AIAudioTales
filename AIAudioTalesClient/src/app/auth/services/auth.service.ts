import { Injectable } from '@angular/core';
import { environment } from 'src/environment/environment';
import { HttpClient, HttpHeaderResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterUser } from 'src/app/entities';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private path = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  public signOutExternal(){
    localStorage.removeItem("user");
  }

  public login(email: string, password: string): Observable<any>{
    const user = {
      "email": email,
      "password": password
    };
    return this.httpClient.post(
      this.path + "Auth/Login", user, {withCredentials: true}
    );
  }
  public register(user: RegisterUser):Observable<any>{
    return this.httpClient.post(this.path+"Auth/Register", user );
  }

  public loginWithGoogle(credentials: string): Observable<any>{
    const headers = new HttpHeaders().set('Content-type', 'application/json');

    const requestOptions = {
      headers: headers,
      withCredentials: true
    }
    return this.httpClient.post(
      this.path + "Auth/LoginWithGoogle",
      JSON.stringify(credentials),
      requestOptions
    );
  }

  refreshToken(): Observable<any> {
    return this.httpClient.get(this.path + "Auth/RefreshToken");
  }

  revokeToken(): Observable<any> {
    return this.httpClient.delete(this.path + "Auth/RevokeToken");
  }

}
