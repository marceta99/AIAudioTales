import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { Book, BookCategory, BooksPaginated } from 'src/app/entities';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.scss']
})
export class BooksComponent implements OnInit {
  booksPaginated: BooksPaginated[] = [];

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    for(const category in BookCategory){
      if(!isNaN(Number(category))){
        this.loadBooksFromCategory(category, 1, 6)
      }
    }
  }

  loadBooksFromCategory(bookCategory: string, pageNumber: number, pageSize: number): void{
    this.bookService.getBooksFromCategory(Number(bookCategory), pageNumber, pageSize).subscribe({
      next: (books : Book[] ) => {
        console.log(books)
        const bookPaginated : BooksPaginated = {
          booksCategory: Number(bookCategory) as BookCategory,
          books: books,
          pageSize: pageSize,
          pageNumber: pageNumber
        }
        this.booksPaginated.push(bookPaginated);
    },
      error: error => {
        console.error('There was an error!', error);
    }
    })
  }

  loadNextPage(bookCategory: BookCategory, pageNumber: number, pageSize: number): void{

  }
  loadPreviousPage(bookCategory: BookCategory, pageNumber: number, pageSize: number): void{

  }
}
