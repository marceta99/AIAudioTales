import { Component, OnInit } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { BookService } from '../services/book.service';
import { Book } from 'src/app/entities';
import { Router } from '@angular/router';

@Component({
  selector: 'app-my-books',
  templateUrl: './my-books.component.html',
  styleUrls: ['./my-books.component.scss']
})
export class MyBooksComponent implements OnInit {
  public books!: Book[];

  constructor(private spinnerService: LoadingSpinnerService,
              private bookService: BookService,
              private router: Router) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
  
    this.bookService.getCreatorBooks().subscribe((books: Book[])=> this.books = books);
  }
  navigateToTree(bookId: number){
    this.router.navigate(['/home/book-tree',bookId])
  }
}
