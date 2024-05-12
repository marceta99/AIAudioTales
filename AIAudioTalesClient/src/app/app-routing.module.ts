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
import { DiscoverComponent } from './home/discover/discover.component';
import { BasketComponent } from './home/basket/basket.component';
import { RegisterCreatorComponent } from './auth/register-creator/register-creator.component';
import { StatisticsComponent } from './home/statistics/statistics.component';
import { CreateBookComponent } from './home/create-book/create-book.component';
import { MyBooksComponent } from './home/my-books/my-books.component';
import { BookTreeComponent } from './home/book-tree/book-tree.component';

const routes: Routes = [
  {path:"login", component : LoginComponent},
  {path:"register", component : RegisterComponent},
  {path:"register-creator", component : RegisterCreatorComponent},
  {path:"home", component : HomeComponent, canActivate:[IsUserLoginGuard], children :
  [
    {path: "", component: BooksComponent},
    {path: "books/:bookId", component : BookComponent},

    {path: "library", component: LibraryComponent},
    {path: "library/player/:bookId", component: PlayerComponent},

    {path: "my-profile", component: MyProfileComponent},
    {path: "discover", component: DiscoverComponent},
    {path: "basket", component: BasketComponent},

    {path: "statistics", component: StatisticsComponent},
    {path: "create-book", component: CreateBookComponent},
    {path: "my-books", component: MyBooksComponent},
    {path: "book-tree/:bookId", component: BookTreeComponent},
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
