
import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { Book, BookCategory, BooksPaginated } from 'src/app/entities';

@Component({
  selector: 'app-testslider',
  templateUrl: './testslider.component.html',
  styleUrls: ['./testslider.component.scss']
})
export class TestsliderComponent implements OnInit{

  booksPaginated: BooksPaginated[] = [];
  homePageCategories : number[] = [5, 6];
  constructor(private bookService: BookService) {}

  ngOnInit(): void {
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
    },
      error: error => {
        console.error('There was an error!', error);
    }
    })
  }

}
