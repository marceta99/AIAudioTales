import { Route } from "@angular/router";
import { HomeComponent } from "./home.component";
import { canActivateListener } from "./guards/listener.guard";
import { toggleSearchGuard } from "./guards/toggle-search.guard";
import { SearchService } from "./services/search.service";

export default [
    {
      path: "", 
      component: HomeComponent,
      providers: [SearchService], // search service is provided here in route injector so that it can be injected inside toggleSearchGuard
      canActivate: [canActivateListener],
      children: [
        {
          path: "",
          loadComponent: () => import("./components/catalog/catalog.component").then(m => m.CatalogComponent)
        },
        {
          path: "search",
          loadComponent: () => import("./components/search/search.component").then(m => m.SearchComponent),
          canDeactivate: [toggleSearchGuard]
        },
        {
          path: "books/:bookId",
          loadComponent: () => import("./components/book/book.component").then(m => m.BookComponent)
        },
        {
          path: "discover",
          loadComponent: () => import("./components/discover/discover.component").then(m => m.DiscoverComponent)
        },
        {
          path: "library",
          loadComponent: () => import("./components/library/library.component").then(m => m.LibraryComponent)
        },
        {
          path: 'my-profile',
          loadChildren: () => import("./components/my-profile/my-profile.routes")
        },
      ]
    },
    
] satisfies Route[];