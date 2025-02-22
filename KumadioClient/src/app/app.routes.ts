import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'home',
    loadChildren: () => import("./home/home.routes")
  },
  {
    path: "login",
    loadComponent: () => import("./auth/login/login.component").then(m => m.LoginComponent)
  },
  {
    path: "register",
    loadComponent: () => import("./auth/register/register.component").then(m => m.RegisterComponent)
  },
  {
    path: "register-creator",
    loadComponent: () => import("./auth/register-creator/register-creator.component").then(m => m.RegisterCreatorComponent)
  },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'not-found'
  },
];
