import { Component } from '@angular/core';
import { BookService } from '../services/book.service';
import { Book, User } from 'src/app/entities';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/auth/services/auth.service';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.scss']
})
export class BookComponent {
  book! : Book;
  currentUser!: User;
  constructor(private bookService: BookService, private route: ActivatedRoute, private authService: AuthService) {}

  ngOnInit():void{
    this.route.params.subscribe(params => {
      const id = +params['bookId'];

      this.bookService.getBookWithId(id).subscribe({
        next: (book: Book) => {
          console.log(book);
          this.book = book;
        },
        error: (error: any) => {
          console.log(error);
        }
      });
    });

  this.currentUser = this.authService.loggedUser;
 }

}
