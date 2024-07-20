import { ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
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
  bookIndex = 0;
  isPlaying = false;
  questionsActive = false;
  showMusicList = true;
  progress = 0;
  currentTime = '0:00';
  maxDuration = '0:00';
  @ViewChild('audioElement', { static: false }) audioElement!: ElementRef;

  constructor(private bookService: BookService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    this.bookService.getPurchasedBooks().subscribe({
      next: (books : PurchasedBook[] ) => {
        console.log(books);
        this.books = books;  
        this.setCurrentBook();
         
        // Detect changes to ensure ViewChild audioElement is updated
        this.cdr.detectChanges();
        this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
        this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
        this.updateDuration();
    },
      error: error => {
        console.error('There was an error!', error);
    }
    })
  }

  setCurrentBook(): void{
    let foundBook = null;
    let foundIndex = -1;

    for (let i = 0; i < this.books.length; i++) {
      if (this.books[i].isBookPlaying) {
        foundBook = this.books[i];
        foundIndex = i;
        break; // Exit loop once the book is found
      }
    }

    // If a playing book was found, set currentBook and bookIndex
    if (foundBook) {
      this.currentBook = foundBook;
      this.bookIndex = foundIndex;
    } else {
      // If user never played any books, then set initial currentBook to the first book in books array
      this.currentBook = this.books[0];
      this.bookIndex = 0;
    }
  }

  loadBook(index: number) {
    this.currentBook = this.books[index];
    this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
    this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
    this.updateDuration();
  }

  togglePlayPause() {
    if (this.isPlaying) {
      this.audioElement.nativeElement.pause();
      this.saveProgress(); // save playing progress when pause is clicked
    } else {
      this.audioElement.nativeElement.play();
    }
    this.isPlaying = !this.isPlaying;
  }

  nextBook() {
    this.saveProgress();

    this.bookIndex++;
    if (this.bookIndex > this.books.length - 1) this.bookIndex = 0;
    this.loadBook(this.bookIndex);
    this.updateProgress();
    if (this.isPlaying) this.audioElement.nativeElement.play();
  }

  prevBook() {
    this.saveProgress(); //save progress of currentBook

    this.bookIndex--;
    if (this.bookIndex < 0) this.bookIndex = this.books.length -1;
    this.loadBook(this.bookIndex);
    this.updateProgress();
    if (this.isPlaying) this.audioElement.nativeElement.play();
  }

  playSelectedSong(index: number) {
    this.saveProgress(); //save progress of last play

    console.log("play selected song index", index)
    this.bookIndex = index;
    this.loadBook(this.bookIndex);
    this.audioElement.nativeElement.play();
    this.isPlaying = true;
  }

  updateProgress() {
    const current = this.audioElement.nativeElement.currentTime;
    const duration = this.audioElement.nativeElement.duration;
    this.progress = (current / duration) * 100;
    console.log("update progress", this.progress)
    this.currentTime = this.formatTime(current);
    this.maxDuration = this.formatTime(duration);
  }

  updateDuration() {
    this.audioElement.nativeElement.onloadedmetadata = () => {
      this.maxDuration = this.formatTime(this.audioElement.nativeElement.duration);
    }
  }

  formatTime(seconds: number) {
    const min = Math.floor(seconds / 60);
    const sec = Math.floor(seconds % 60);
    return `${min}:${sec < 10 ? '0' : ''}${sec}`;
  }

  toggleMusicList() {
    this.showMusicList = !this.showMusicList;
  }

  nextPart(nextPlayingPartId: number | null){
    const bookId = this.currentBook.id;
    const nextPartId = nextPlayingPartId as number;

    this.bookService.nextPart(bookId, nextPartId).subscribe((updatedPurchasedBook: PurchasedBook)=>{
      this.currentBook = updatedPurchasedBook;
      this.books[this.bookIndex] = updatedPurchasedBook;

      this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
      this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
      this.updateProgress();
      this.questionsActive = false;
      this.isPlaying = true;
      this.audioElement.nativeElement.play();
    })
  }

  loadQuestions(): void{
    if(this.currentBook.playingPart.answers.length > 0){
      this.isPlaying = false;
      this.questionsActive = true;
      this.bookService.activateQuestions(this.currentBook.id).subscribe(()=>{
        console.log("questions activated")
      })
    }else {
      //if there is no more answers it means that user have reached the end of that book part and we should
      // set current book back to start and play the next book
      this.bookService.startBookAgain(this.currentBook.id).subscribe({
        next: (purchasedBook: PurchasedBook) => {
          console.log("start again resposne ", purchasedBook)
          this.books[this.bookIndex] = purchasedBook;
          
          //load next book when this is finished
          this.bookIndex++;
          if (this.bookIndex > this.books.length - 1) this.bookIndex = 0;
          this.loadBook(this.bookIndex);
          if (this.isPlaying) this.audioElement.nativeElement.play();
        },
        error: (error) => console.error('Error updating progress', error)
      });
    }
  }

  saveProgress(): void{
    const currentTimeSec = this.audioElement.nativeElement.currentTime;
    const bookId = this.currentBook.id;
    this.currentBook.playingPosition = currentTimeSec; 

    this.bookService.updateProgress(bookId, currentTimeSec).subscribe({
      next: () => { 
        console.log('Progress updated successfully')
      },
      error: (error) => console.error('Error updating progress', error)
    });
  }
}
