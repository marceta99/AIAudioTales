import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { ActivatedRoute } from '@angular/router';
import { PurchasedBook, User } from 'src/app/entities';
import { AuthService } from 'src/app/auth/services/auth.service';

@Component({
  selector: 'app-library',
  templateUrl: './library.component.html',
  styleUrls: ['./library.component.scss']
})
export class LibraryComponent implements OnInit{
  currentUser!: User;
  books!: PurchasedBook[];

  constructor(private bookService: BookService, private authService: AuthService) {}

  ngOnInit(): void {
    this.currentUser = this.authService.loggedUser;

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
