import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { Book, Language, Purchase, PurchaseType, User } from 'src/app/entities';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/auth/services/auth.service';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.scss']
})
export class BookComponent implements OnInit{
  book! : Book;
  currentUser!: User;
  userHasBook: boolean = false;
  constructor(
    private bookService: BookService,
    private route: ActivatedRoute,
    private authService: AuthService,
    private router: Router) {}

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
    const purchase : Purchase = {
      bookId : this.book.id,
      language: Language.ENGLISH_USA,
      purchaseType: purchaseType
    }
    this.bookService.purchaseBook(purchase).subscribe({
      next: () => {
        this.router.navigate(['/home/library/player',this.book.id])
    },
      error: (error : Error) => {
        console.log(error)
    }
    })
  }

}
