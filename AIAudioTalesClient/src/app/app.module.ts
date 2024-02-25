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
import { SliderComponent } from './home/slider/slider.component';
import { LibraryComponent } from './home/library/library.component';
import { MyProfileComponent } from './home/my-profile/my-profile.component';
import { PlayerComponent } from './home/library/player/player.component';
import { ToastNotificationComponent } from './home/toast-notification/toast-notification.component';
import { LoadingSpinnerComponent } from './home/loading-spinner/loading-spinner.component';
import { DiscoverComponent } from './home/discover/discover.component';
import { SearchBarComponent } from './home/discover/search-bar/search-bar.component';
import { BasketComponent } from './home/basket/basket.component';
import { SuccessPurchaseComponent } from './home/basket/success-purchase/success-purchase.component';
import { CancelPurchaseComponent } from './home/basket/cancel-purchase/cancel-purchase.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomeComponent,
    BookComponent,
    SidenavComponent,
    BooksComponent,
    BookCategoryPipe,
    SliderComponent,
    LibraryComponent,
    MyProfileComponent,
    PlayerComponent,
    ToastNotificationComponent,
    LoadingSpinnerComponent,
    DiscoverComponent,
    SearchBarComponent,
    BasketComponent,
    SuccessPurchaseComponent,
    CancelPurchaseComponent
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
