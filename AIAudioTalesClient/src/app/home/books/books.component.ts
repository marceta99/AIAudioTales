import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { Book, BookCategory } from 'src/app/entities';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.scss']
})
export class BooksComponent implements OnInit {
  bedTimeBooks!: Book[];
  natureBooks!: Book[];
  historyBooks!: Book[];

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.bookService.getBooksFromCategory(BookCategory.BedTime, 1, 10).subscribe({
      next: (books : Book[] ) => {
        console.log(books);
        this.bedTimeBooks = books ;
    },
      error: error => {
        console.error('There was an error!', error);
    }
    })

    this.bookService.getBooksFromCategory(BookCategory.Nature, 1, 10).subscribe({
      next: (books : Book[] ) => {
        console.log(books);
        this.natureBooks = books ;
    },
      error: error => {
        console.error('There was an error!', error);
    }
    })

    this.bookService.getBooksFromCategory(BookCategory.History, 1, 10).subscribe({
      next: (books : Book[] ) => {
        console.log(books);
        this.historyBooks = books ;
    },
      error: error => {
        console.error('There was an error!', error);
    }
    })
  }

}
