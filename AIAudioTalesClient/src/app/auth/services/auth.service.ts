import { Injectable } from '@angular/core';
import { environment } from 'src/environment/environment';
import { HttpClient, HttpHeaderResponse, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private path = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  public signOutExternal(){
    localStorage.removeItem("user");
  }

  public login(email: String, password: String): Observable<any>{
    const user = {
      "userName": email,
      "password": password
    };
    return this.httpClient.post(
      this.path + "Auth/Login", user, {withCredentials : true}
    );
  }

  public loginWithGoogle(credentials: string): Observable<any>{
    const header = new HttpHeaders().set('Content-type', 'application/json');
    return this.httpClient.post(
      this.path + "Auth/LoginWithGoogle",
      JSON.stringify(credentials),
      { headers: header}
    );
  }

  public getColors():Observable<any>{
    return this.httpClient.get(this.path + "Item/GetColorList",
    {withCredentials : true});
  }

  refreshToken(): Observable<any> {
    const header = new HttpHeaders().set('Content-type', 'application/json');
    return this.httpClient.get(this.path + "Auth/RefreshToken", { headers: header, withCredentials: true });
  }

  revokeToken(): Observable<any> {
    const header = new HttpHeaders().set('Content-type', 'application/json');
    return this.httpClient.delete(this.path + "Auth/RevokeToken/marcetic.mihailo99@gmail.com" , { headers: header, withCredentials: true });
  }

}
