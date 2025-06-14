import { CommonModule } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { PurchasedBook } from 'src/app/entities';
import { LibraryService } from '../../services/library.service';
import { PlayerService } from '../../services/player.service';

@Component({
  selector: 'app-library',
  templateUrl: './library.component.html',
  styleUrls: ['./library.component.scss'],
  imports: [CommonModule]
})
export class LibraryComponent implements OnInit{
  books: PurchasedBook[] = [];
  currentBookIndex!: number;
  isPlaying: boolean = false;

  constructor(
    private libraryService: LibraryService,
    private playerService: PlayerService,
    private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {

    this.libraryService.getPurchasedBooks().subscribe({
      next: (books : PurchasedBook[] ) => {
        console.log(books);
        this.books = books;
        this.setCurrentBook()  
      },
      error: error => {
          console.error('There was an error!', error);
      }
    })

    this.libraryService.purchasedBooks.subscribe((books: PurchasedBook[])=>{
      console.log("purchased books observable")
      this.books = books;
      this.setCurrentBook();    // <— highlight the right index
    })

    this.playerService.currentBookIndex.subscribe((index: number)=>{
      console.log("current book index updated")
      this.currentBookIndex = index;
    })

    this.playerService.isPlaying.subscribe((isPlaying: boolean)=> this.isPlaying = isPlaying);

      // Subscribe to remaining time and update the displayed time for the current book
    this.playerService.remainingTime.subscribe((timeLeft: number) => {
      console.log("remainning time ", timeLeft)
      if (this.books && this.books[this.currentBookIndex]) {
        this.books[this.currentBookIndex].remainingTime = timeLeft;
        this.cdr.detectChanges(); // Trigger change detection to update the view
        console.log("curent book ", this.books[this.currentBookIndex])
      }
    });
  }

  setCurrentBook(): void{
    for (let i = 0; i < this.books.length; i++) {
      if (this.books[i].isBookPlaying) {
        this.currentBookIndex  = i;
        break; // Exit loop once the book is found
      }
    }
  }

  playSelectedBook(index: number) {
    console.log("play this book", index);
    this.playerService.currentBookIndex.next(index);
  }
  
}
