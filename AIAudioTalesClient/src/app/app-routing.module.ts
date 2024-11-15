import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { HomeComponent } from './home/home.component';
import { BookComponent } from './home/book/book.component';
import { BooksComponent } from './home/books/books.component';
import { IsUserLoginGuard } from './guards/is-user-login.guard';
import { LibraryComponent } from './home/library/library.component';
import { MyProfileComponent } from './home/my-profile/my-profile.component';
import { DiscoverComponent } from './home/discover/discover.component';
import { BasketComponent } from './home/basket/basket.component';
import { RegisterCreatorComponent } from './auth/register-creator/register-creator.component';
import { StatisticsComponent } from './home/statistics/statistics.component';
import { CreateBookComponent } from './home/create-book/create-book.component';
import { MyBooksComponent } from './home/my-books/my-books.component';
import { BookTreeComponent } from './home/book-tree/book-tree.component';
import { AccountComponent } from './home/my-profile/settings/account/account.component';
import { LanguageComponent } from './home/my-profile/settings/language/language.component';
import { ListenHistoryComponent } from './home/my-profile/settings/listen-history/listen-history.component';
import { SupportComponent } from './home/my-profile/settings/support/support.component';
import { AchievementsComponent } from './home/my-profile/settings/achievements/achievements.component';
import { SearchComponent } from './home/search/search.component';

const routes: Routes = [
  {path:"login", component : LoginComponent},
  {path:"register", component : RegisterComponent},
  {path:"register-creator", component : RegisterCreatorComponent},
  {path:"home", component : HomeComponent, canActivate:[IsUserLoginGuard], children :
  [
    {path: "", component: BooksComponent},
    {path: "books/:bookId", component : BookComponent},

    {path: "library", component: LibraryComponent},

    { path: 'my-profile', component: MyProfileComponent, children: [
      { path: 'account', component: AccountComponent },
      { path: 'language', component: LanguageComponent },
      { path: 'listen-history', component: ListenHistoryComponent },
      { path: 'support', component: SupportComponent },
      { path: 'achievements', component: AchievementsComponent },
      { path: '', redirectTo: 'account', pathMatch: 'full' } // Default tab
    ]},
    {path: "discover", component: DiscoverComponent},
    {path: "basket", component: BasketComponent},
    {path: "search", component: SearchComponent},

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
