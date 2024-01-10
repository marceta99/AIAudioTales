import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, catchError, map, of } from 'rxjs';
import { AuthService } from '../auth/services/auth.service';
import { User } from '../entities';

@Injectable({
  providedIn: 'root'
})
export class IsUserLoginGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

      if(this.authService.isLoggedIn)return true;

      return this.authService.getCurrentUser().pipe(
        map((user) => {
          // If the user is obtained successfully
          console.log(user)
          this.authService.isLoggedIn = true;
          this.authService.loggedUser = user;
          return true;
        }),
        catchError((error) => {
          // On error or if user is not logged in
          console.log(error);
          this.router.navigate(['/login']).then(()=> window.location.reload());
          return of(false);
        })
      );
  }

}
