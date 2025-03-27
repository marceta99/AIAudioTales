import { CanDeactivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { SearchComponent } from '../components/search/search.component';
import { SearchService } from '../services/search.service';

export const toggleSearchGuard: CanDeactivateFn<SearchComponent> = (
  component: SearchComponent,
  currentRoute: ActivatedRouteSnapshot,
  currentState: RouterStateSnapshot,
  nextState?: RouterStateSnapshot
): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree => {
  const searchService = inject(SearchService);

  // When the user leaves the search route, set the search toggle to false.
  searchService.isSearchActive.next(false);

  return true;
};
