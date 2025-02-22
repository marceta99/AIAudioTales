import { Route } from "@angular/router";
import { HomeComponent } from "./home.component";

export default [
    {path: "", component: HomeComponent, children: [
        {
          path: "",
          loadComponent: () => import("./components/catalog/catalog.component").then(m => m.CatalogComponent)
        },
        // You can add additional child routes here:
        // { path: 'another-route', loadComponent: () => import('./another/another.component').then(m => m.AnotherComponent) }
      ]
    },
    
] satisfies Route[];