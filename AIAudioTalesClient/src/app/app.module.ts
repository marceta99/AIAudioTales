import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HomeComponent } from './home/home.component';
import { BookComponent } from './home/book/book.component';
import { AuthInterceptor } from './auth/services/auth.interceptor';
import { SidenavComponent } from './home/sidenav/sidenav.component';
import { BooksComponent } from './home/books/books.component';
import { BookCategoryPipe } from './pipes/category.pipe';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    BookComponent,
    SidenavComponent,
    BooksComponent,
    BookCategoryPipe
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true,
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
