import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { Basket, Book, Language, Purchase, PurchaseType, ReturnBook, Toast, ToastIcon, ToastType, User } from 'src/app/entities';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/auth/services/auth.service';
import { ToastNotificationService } from '../services/toast-notification.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.scss']
})
export class BookComponent implements OnInit{
  book! : ReturnBook;
  currentUser!: User | null;
  userHasBook: boolean = false;
  isBasketItem: boolean = false;
  disableButton: boolean = false;
  freeBook: boolean = true;

  constructor(
    private bookService: BookService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private location: Location,
    private notificationService: ToastNotificationService) {}

  ngOnInit():void{
    this.route.params.subscribe(params => {
      const id = +params['bookId'];

      this.bookService.getBookWithId(id).subscribe({
        next: (book: ReturnBook) => {
          console.log(book);
          this.book = book;

          this.bookService.userHasBook(this.book.id).subscribe({
            next: (hasBook: boolean) => {
              this.userHasBook = hasBook;
            },
            error: (error: any) => {
              console.log(error);
            }
          });
          this.bookService.isBasketItem(this.book.id).subscribe({
            next: (isBasketItem: boolean) => {
              this.isBasketItem = isBasketItem;
            },
            error: (error: any) => {
              console.log(error);
            }
          });
        },
        error: (error: any) => {
          console.log(error);
        }
      });
    });

    this.authService.currentUser.subscribe(user=> {this.currentUser = user})
  }

  purchaseBook(purchaseType: PurchaseType){
    this.disableButton = true;
    const purchase : Purchase = {
      bookId : this.book.id,
      language: Language.ENGLISH_USA,
      purchaseType: purchaseType
    }
    this.bookService.purchaseBook(purchase).subscribe({
      next: () => {
        this.userHasBook = true;
        const toast: Toast = {
          text: "Successfully purchased book",
          toastIcon: ToastIcon.Success,
          toastType: ToastType.Success
        }
        this.notificationService.show(toast);
    },
      error: (error : Error) => {
        console.log(error)
        this.disableButton = false;
        const toast: Toast = {
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error
        }
        this.notificationService.show(toast);
    }
    })
  }

  addBasketItem(bookId: number){
    this.disableButton = true;
    this.bookService.addBasketItem(bookId).subscribe({
      next: (basket: Basket) => {
        this.bookService.basket.next(basket);
        const toast: Toast = {
          text: "Added to basket",
          toastIcon: ToastIcon.Success,
          toastType: ToastType.Success
        }
        this.notificationService.show(toast);
        this.isBasketItem = true;
        this.disableButton = false;

    },
      error: (error : Error) => {
        console.log(error)
        this.disableButton = false;
        const toast: Toast = {
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error
        }
        this.notificationService.show(toast);
    }
    })
  }

  goBack(): void {
    this.location.back(); // Go back to the previous route
  }

  hasPreviousRoute(): boolean {
    return !!window.history.state.navigationId; // Check if there is a previous route
  }

  addToLibrary(): void { 
    this.bookService.addToLibrary(this.book.id).subscribe({
      next: () => {
        this.userHasBook = true;
        const toast: Toast = {
          text: "Added to basket",
          toastIcon: ToastIcon.Success,
          toastType: ToastType.Success
        }
        this.notificationService.show(toast);

    },
      error: (error : Error) => {
        console.log(error)
        const toast: Toast = {
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error
        }
        this.notificationService.show(toast);
    }
    })
  }
}
