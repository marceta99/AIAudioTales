import { Component } from '@angular/core';
import { BookService } from '../services/book.service';
import { Book } from 'src/app/entities';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.scss']
})
export class BookComponent {
  book! : Book;

  constructor(private bookService: BookService, private route: ActivatedRoute) {}

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

 }

}
