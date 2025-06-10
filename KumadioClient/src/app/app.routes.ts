import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'home',
    loadChildren: () => import("./home/home.routes")
  },
  {
    path: 'creator',
    loadChildren: () => import("./creator-home/creator.routes")
  },
  {
    path: 'onboarding',
    loadComponent: () => import('./onboarding/onboarding.component').then(m => m.OnboardingComponent),
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
    path: "email-confirmation-sent",
    loadComponent: () => import("./auth/email-confirmation-sent/email-confirmation-sent.component").then(m => m.EmailConfirmationSentComponent)
  },
   {
    path: "confirm-email",
    loadComponent: () => import("./auth/email-confirm/email-confirm.component").then(m => m.ConfirmEmailComponent)
  },
  {
    path: "register-creator",
    loadComponent: () => import("./auth/register-creator/register-creator.component").then(m => m.RegisterCreatorComponent)
  },
  {
    path: "not-found",
    loadComponent: () => import("./common/components/not-found/not-found.component").then(m => m.NotFoundComponent)
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
