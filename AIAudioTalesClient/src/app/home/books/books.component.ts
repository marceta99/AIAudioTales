import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { BookCategory, ReturnBook } from 'src/app/entities';
import { LoadingSpinnerService } from '../services/loading-spinner.service';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.scss']
})
export class BooksComponent implements OnInit {
  homePageCategories : number[] = [5, 6];
  categories: { books: ReturnBook[], category: BookCategory }[] = [];

  constructor(private bookService: BookService, private spinnerService: LoadingSpinnerService) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(true);

    this.homePageCategories.forEach(category =>{
      this.loadBooksFromCategory(category, 1, 15)
   })
   
   this.spinnerService.setLoading(false);
  }

  loadBooksFromCategory(bookCategory: number, pageNumber: number, pageSize: number): void {
    this.bookService.getBooksFromCategory(bookCategory, pageNumber, pageSize).subscribe({
      next: (books : ReturnBook[] ) => {
        console.log(books)
        const category = { 
          books : books,
          category: bookCategory as BookCategory
        } ;
        this.categories.push(category);
    },
      error: error => {
        console.error('There was an error!', error);
        this.spinnerService.setLoading(false);
    }
    })
  }
}
