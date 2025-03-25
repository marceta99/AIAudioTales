import { inject } from "@angular/core";
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from "@angular/router";
import { map, catchError, of } from "rxjs";
import { AuthService } from "src/app/auth/services/auth.service";
import { User, Role } from "src/app/entities";

export const canActivateListener: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const authService = inject(AuthService)
  const router = inject(Router)

  const currentUser = authService.currentUser;
  if (currentUser) {
    return checkListenerRole(currentUser) ? true : router.parseUrl('/login');
  }

  return authService.getCurrentUser().pipe(
    map((user: User) => {
      return checkListenerRole(user) ? true : router.parseUrl('/login');
    }),
    catchError((error) => {
      // Not logged in or fetch failed => redirect
      console.log(error);
      router.navigate(['/login']);
      return of(false);
    })
  );

};

function checkListenerRole(user: User): boolean {
  return (
    user.role === Role.LISTENER_NO_SUBSCRIPTION ||
    user.role === Role.LISTENER_WITH_SUBSCRIPTION
  );
}