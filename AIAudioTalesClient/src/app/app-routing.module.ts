import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { HomeComponent } from './home/home.component';
import { BookComponent } from './home/book/book.component';

const routes: Routes = [
  {path:"login", component : LoginComponent},
  {path:"register", component : RegisterComponent},
  {path:"home", component : HomeComponent, children :
  [
    { path: 'books/:bookId', component : BookComponent},
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
