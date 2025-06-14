import { Route } from "@angular/router";
import { MyProfileComponent } from "./my-profile.component";

export default [
    {path: "", component: MyProfileComponent, children: [
      {
        path: "",
        loadComponent: () =>
          import("./my-profile.component").then(m => m.MyProfileComponent)
      },
        {
          path: "account",
          loadComponent: () => import("./settings/account/account.component").then(m => m.AccountComponent)
        },
        {
          path: "language",
          loadComponent: () => import("./settings/language/language.component").then(m => m.LanguageComponent)
        },
        {
          path: "listen-history",
          loadComponent: () => import("./settings/listen-history/listen-history.component").then(m => m.ListenHistoryComponent)
        },
        {
          path: "support",
          loadComponent: () => import("./settings/support/support.component").then(m => m.SupportComponent)
        },
        {
          path: "achievements",
          loadComponent: () => import("./settings/achievements/achievements.component").then(m => m.AchievementsComponent)
        },
     
      ]
    },
    
] satisfies Route[];