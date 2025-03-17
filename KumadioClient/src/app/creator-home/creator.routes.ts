import { Route } from "@angular/router";
import { CreatorHomeComponent } from "./creator-home.component";

export default [
    {path: "", component: CreatorHomeComponent, children: [
        {
          path: "",
          loadComponent: () => import("./components/analytics/analytics.component").then(m => m.AnalyticsComponent)
        },
        {
          path: "create-book",
          loadComponent: () => import("./components/create-book/create-book.component").then(m => m.CreateBookComponent)
        },
        {
          path: "book-tree/:bookId",
          loadComponent: () => import("./components/book-tree/book-tree.component").then(m => m.BookTreeComponent)
        },
        {
          path: "my-books",
          loadComponent: () => import("./components/my-books/my-books.component").then(m => m.MyBooksComponent)
        },
        {
          path: "profile",
          loadComponent: () => import("./components/creator-profile/creator-profile.component").then(m => m.CreatorProfileComponent)
        },
      ]
    },
    
] satisfies Route[];