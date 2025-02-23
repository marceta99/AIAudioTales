import { Route } from "@angular/router";
import { HomeComponent } from "./home.component";

export default [
    {path: "", component: HomeComponent, children: [
        {
          path: "",
          loadComponent: () => import("./components/catalog/catalog.component").then(m => m.CatalogComponent)
        },
        {
          path: "search",
          loadComponent: () => import("./components/search/search.component").then(m => m.SearchComponent)
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