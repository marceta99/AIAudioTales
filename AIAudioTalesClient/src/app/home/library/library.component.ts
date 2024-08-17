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
  currentBookIndex!: number;

  constructor(private bookService: BookService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {

    this.bookService.getPurchasedBooks().subscribe({
      next: (books : PurchasedBook[] ) => {
        console.log(books);
        this.books = books;
        this.setCurrentBook()  
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

  setCurrentBook(): void{
    for (let i = 0; i < this.books.length; i++) {
      if (this.books[i].isBookPlaying) {
        this.currentBookIndex  = i;
        break; // Exit loop once the book is found
      }
    }
  }

  playSelectedSong(index: number) {
    console.log("play this book", index);
    this.bookService.currentBookIndex.next(index);
    /*this.saveProgress(this.books[index].id); //save progress of last play

    console.log("play selected song index", index)
    this.bookIndex = index;
    this.loadBook(this.bookIndex);*/
  }
  
}
