import { ChangeDetectorRef, Component, ElementRef, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BookService } from '../services/book.service';
import { PurchasedBook } from 'src/app/entities';

@Component({
  selector: 'app-library',
  templateUrl: './library.component.html',
  styleUrls: ['./library.component.scss']
})
export class LibraryComponent implements OnInit{
  books!: PurchasedBook[];
  currentBook!: PurchasedBook;
  currentBookIndex!: number;

  constructor(private bookService: BookService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {

    this.bookService.getPurchasedBooks().subscribe({
      next: (books : PurchasedBook[] ) => {
        console.log(books);
        this.books = books;  
      },
      error: error => {
          console.error('There was an error!', error);
      }
    })

    this.bookService.purchasedBooks.subscribe((books: PurchasedBook[])=>{
      this.books = books;
    })

    this.bookService.currentBookIndex.subscribe((index: number)=>{
      this.currentBookIndex = index;
    })
  }

  playSelectedSong(index: number) {
    console.log("play this book", index);
    /*this.saveProgress(this.books[index].id); //save progress of last play

    console.log("play selected song index", index)
    this.bookIndex = index;
    this.loadBook(this.bookIndex);*/
  }
  
}
