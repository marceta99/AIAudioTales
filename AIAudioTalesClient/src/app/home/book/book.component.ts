import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { Book, Language, Purchase, PurchaseType, Toast, ToastIcon, ToastType, User } from 'src/app/entities';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/auth/services/auth.service';
import { ToastNotificationService } from '../services/toast-notification.service';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.scss']
})
export class BookComponent implements OnInit{
  book! : Book;
  currentUser!: User;
  userHasBook: boolean = false;
  disableButton: boolean = false;
  constructor(
    private bookService: BookService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router,
    private toastNotificationService: ToastNotificationService) {}

  ngOnInit():void{
    this.route.params.subscribe(params => {
      const id = +params['bookId'];

      this.bookService.getBookWithId(id).subscribe({
        next: (book: Book) => {
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

        },
        error: (error: any) => {
          console.log(error);
        }
      });
    });

    this.currentUser = this.authService.loggedUser;
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
        this.toastNotificationService.show(toast);
    },
      error: (error : Error) => {
        console.log(error)
        this.disableButton = false;
        const toast: Toast = {
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error
        }
        this.toastNotificationService.show(toast);
    }
    })
  }

}
