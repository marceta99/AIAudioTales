import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { PurchasedBook, User } from 'src/app/entities';
import { AuthService } from 'src/app/auth/services/auth.service';

@Component({
  selector: 'app-library',
  templateUrl: './library.component.html',
  styleUrls: ['./library.component.scss']
})
export class LibraryComponent implements OnInit{
  currentUser!: User | null;
  books!: PurchasedBook[];

  constructor(private bookService: BookService, private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser.subscribe(user=> {this.currentUser = user})

    this.bookService.getUserBooks().subscribe({
      next: (books : PurchasedBook[] ) => {
        console.log(books);
        this.books = books;
    },
      error: error => {
        console.error('There was an error!', error);
    }
    })
  }

}
