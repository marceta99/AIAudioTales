import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { Book, BookCategory, BooksPaginated } from 'src/app/entities';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.scss']
})
export class BooksComponent implements OnInit {
  booksPaginated: BooksPaginated[] = [];
  homePageCategories : number[] = [5, 6];

  constructor(private bookService: BookService, private loadingSpinnerService: LoadingSpinnerService) {}

  ngOnInit(): void {
    this.loadingSpinnerService.setLoading(true);

    this.homePageCategories.forEach(category =>{
      this.loadBooksFromCategory(category, 1, 15)
   })
  }

  loadBooksFromCategory(bookCategory: number, pageNumber: number, pageSize: number): void{
    this.bookService.getBooksFromCategory(bookCategory, pageNumber, pageSize).subscribe({
      next: (books : Book[] ) => {
        console.log(books)
        const bookPaginated : BooksPaginated = {
          booksCategory: bookCategory as BookCategory,
          books: books,
          pageSize: pageSize,
          pageNumber: pageNumber
        }
        this.booksPaginated.push(bookPaginated);

        this.loadingSpinnerService.setLoading(false);
    },
      error: error => {
        console.error('There was an error!', error);
        this.loadingSpinnerService.setLoading(false);
    }
    })
  }
}
