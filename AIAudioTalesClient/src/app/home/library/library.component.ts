import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { BookService } from '../services/book.service';
import { PurchasedBook, ReturnPart } from 'src/app/entities';

@Component({
  selector: 'app-library',
  templateUrl: './library.component.html',
  styleUrls: ['./library.component.scss']
})
export class LibraryComponent implements OnInit{
  books!: PurchasedBook[];

  @ViewChild('audioElement', { static: false }) audioElement!: ElementRef;
  currentBook!: PurchasedBook;
  bookIndex = 1;
  isPlaying = false;
  questionsActive = false;
  showMusicList = true;
  progress = 0;
  currentTime = '0:00';
  maxDuration = '0:00';

  constructor(private bookService: BookService, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {

    this.bookService.getPurchasedBooks().subscribe({
      next: (books : PurchasedBook[] ) => {
        console.log(books);
        this.books = books;
        this.currentBook = this.books[this.books.length -1]
        // Detect changes to ensure ViewChild audioElement is updated
        this.cdr.detectChanges();

        this.loadBook(this.books.length)
    },
      error: error => {
        console.error('There was an error!', error);
    }
    })
  }

  loadBook(index: number) {
    this.currentBook = this.books[index - 1];
    this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
    this.updateDuration();
  }

  togglePlayPause() {
    if (this.isPlaying) {
      this.audioElement.nativeElement.pause();
    } else {
      this.audioElement.nativeElement.play();
    }
    this.isPlaying = !this.isPlaying;
  }

  nextBook() {
    this.bookIndex++;
    if (this.bookIndex > this.books.length) this.bookIndex = 1;
    this.loadBook(this.bookIndex);
    if (this.isPlaying) this.audioElement.nativeElement.play();
  }

  prevBook() {
    this.bookIndex--;
    if (this.bookIndex < 1) this.bookIndex = this.books.length;
    this.loadBook(this.bookIndex);
    if (this.isPlaying) this.audioElement.nativeElement.play();
  }

  playSelectedSong(index: number) {
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
    const currentPartId = this.currentBook.playingPart.id;
    const nextPartId = nextPlayingPartId as number;

    this.bookService.nextPart(currentPartId, nextPartId).subscribe((playingPart: ReturnPart)=>{
      this.questionsActive = false;
      this.currentBook.playingPart = playingPart;
      this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
      this.updateDuration();
    })
  }

  loadQuestions(): void{
    if(this.currentBook.playingPart.answers){
      this.isPlaying = false;
      this.questionsActive = true;
    }else {
      //if there is no more answers it means that user have reached the end of that book part and we should play next book
      this.nextBook();
      //this.bookService.startBookAgain()
    }
  }
}
