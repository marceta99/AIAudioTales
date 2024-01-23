import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { HomeComponent } from './home/home.component';
import { BookComponent } from './home/book/book.component';
import { BooksComponent } from './home/books/books.component';
import { IsUserLoginGuard } from './guards/is-user-login.guard';
import { LibraryComponent } from './home/library/library.component';
import { PlayerComponent } from './home/library/player/player.component';
import { MyProfileComponent } from './home/my-profile/my-profile.component';
import { ToastNotificationComponent } from './home/toast-notification/toast-notification.component';
import { LoadingSpinnerComponent } from './home/loading-spinner/loading-spinner.component';
import { DiscoverComponent } from './home/discover/discover.component';

const routes: Routes = [
  {path:"login", component : LoginComponent},
  {path:"register", component : RegisterComponent},
  {path:"home", component : HomeComponent, canActivate:[IsUserLoginGuard], children :
  [
    {path: "", component: BooksComponent},
    {path: "books/:bookId", component : BookComponent},

    {path: "library", component: LibraryComponent},
    {path: "library/player/:bookId", component: PlayerComponent},

    {path: "my-profile", component: MyProfileComponent},
    {path: "discover", component: DiscoverComponent}
  ]
},
  {path:"", redirectTo:"home", pathMatch:"full"},
  {path :'**', redirectTo : 'not-found'},
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
