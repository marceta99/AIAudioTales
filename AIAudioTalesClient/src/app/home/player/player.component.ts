import { ChangeDetectorRef, Component, ElementRef, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { BookService } from '../services/book.service';
import { PurchasedBook } from 'src/app/entities';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit, OnDestroy {
  books!: PurchasedBook[];
  currentBook!: PurchasedBook;
  currentBookIndex = 0;
  recognition: any;
  progress = 0;
  currentTime = '0:00';
  maxDuration = '0:00';
  isPlaying = false;
  fullScreen = false;
  recognitionActive = false;
  isTitleOverflowing: boolean = false;
  isArtistOverflowing: boolean = false;
  @ViewChild('audioElement', { static: false }) audioElement!: ElementRef;  
  @ViewChild('progressArea', { static: false }) progressArea!: ElementRef;
  @ViewChild('titleElement') titleElement!: ElementRef;
  @ViewChild('artistElement') artistElement!: ElementRef;

  
  constructor(private bookService: BookService, private cdr: ChangeDetectorRef, private zone: NgZone) {}

  ngOnInit(): void {
    this.initializeSpeechRecognition();

    this.bookService.getPurchasedBooks().subscribe({
      next: (books : PurchasedBook[] ) => {
        console.log(books);
        this.books = books;  
        this.setCurrentBook();

      },
      error: error => {
          console.error('There was an error!', error);
      }
    });

    this.bookService.currentBookIndex.subscribe((index: number)=>{
      if(this.books){
        this.saveProgress(this.books[index].id);
        this.loadBook(index);
      }
    });

    this.bookService.isPlaying.subscribe((isPlaying: boolean)=> this.isPlaying = isPlaying);
  }

  ngOnDestroy(): void {
    this.stopRecognition();
  }

  setCurrentBook(): void{
    for (let i = 0; i < this.books.length; i++) {
      if (this.books[i].isBookPlaying) {
        this.currentBook = this.books[i]; // msm da ovo mogu da obrisem ovu liniju jer se ovo svakako odradi u loadBook a loadBook se pozove svaki put kada se promeni index, a to je na sledecoj liniji
        this.currentBookIndex  = i;
        this.bookService.currentBookIndex.next(i);
        break; // Exit loop once the book is found
      }
    }
  }

  loadBook(index: number) {
    this.currentBook = this.books[index];
    this.currentBookIndex = index;
    this.cdr.detectChanges();

    this.isTitleOverflowing = this.isOverflowing(this.titleElement.nativeElement);
    this.isArtistOverflowing = this.isOverflowing(this.artistElement.nativeElement);

    if(this.currentBook.questionsActive){
      this.bookService.isPlaying.next(false);
      this.audioElement.nativeElement.pause();
      this.startRecognition();
    }else{
      this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
      this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
      this.updateProgress();
      if (this.isPlaying) this.audioElement.nativeElement.play();
      if (this.recognitionActive) this.stopRecognition(); // stop recognition if was active before
    }

  }

  private isOverflowing(element: HTMLElement): boolean {
    return element.scrollWidth > element.clientWidth;
  }

  updateProgress() {
    const current = this.audioElement.nativeElement.currentTime;
    const duration = this.audioElement.nativeElement.duration;
    this.progress = (current / duration) * 100;
    this.currentTime = this.formatTime(current);
    this.maxDuration = this.formatTime(duration);
  }

  formatTime(seconds: number) {
    const min = Math.floor(seconds / 60);
    const sec = Math.floor(seconds % 60);
    return `${min}:${sec < 10 ? '0' : ''}${sec}`;
  }

  togglePlayPause() {
    // only play something if microphone is not active
    if(!this.currentBook.questionsActive){
      if (this.isPlaying) {
        this.audioElement.nativeElement.pause();
        this.saveProgress(); // save playing progress when pause is clicked
      } else {
        this.audioElement.nativeElement.play();
      }
      this.bookService.isPlaying.next(!this.isPlaying);
    }
  }

  nextBook() {
    this.bookService.currentBookIndex.next(this.currentBookIndex + 1 > this.books.length-1 ? 0 : this.currentBookIndex + 1);
  }

  prevBook() {
    this.bookService.currentBookIndex.next(this.currentBookIndex - 1 < 0 ? this.books.length -1 : this.currentBookIndex - 1);
  }

  saveProgress(nextBookId?: number): void{
    const bookId = this.books[this.currentBookIndex].id;
    let currentTimeSec;
    if(!this.currentBook.questionsActive && this.audioElement){  
      currentTimeSec = this.audioElement.nativeElement.currentTime;
      this.currentBook.playingPosition = currentTimeSec; 
    }
    
    this.bookService.updateProgress(bookId, currentTimeSec, nextBookId).subscribe({
      next: () => { 
        console.log("progress updated successfully")
        this.updateBookList();
      },
      error: (error) => console.error('Error updating progress', error)
    });
  }

  private updateBookList(){
    //update book list with changes of current book object
    this.books[this.currentBookIndex] = this.currentBook;
    this.bookService.purchasedBooks.next(this.books);
  }

  loadQuestions(): void{
    if(this.currentBook.playingPart.answers.length > 0){
      this.bookService.isPlaying.next(false);
      this.currentBook.questionsActive = true;

      this.bookService.activateQuestions(this.currentBook.id).subscribe(()=>{
        console.log("questions activated")
        this.updateBookList();
        this.startRecognition();
      })
    }else {
      //if there is no more answers it means that user have reached the end of that book part and we should
      // set current book back to start and play the next book
      this.bookService.startBookAgain(this.currentBook.id).subscribe({
        next: (updatedPurchasedBook: PurchasedBook) => {
          console.log("start again resposne ", updatedPurchasedBook)
          this.currentBook = updatedPurchasedBook;
          this.updateBookList();

          //load next book when this is finished
          this.currentBookIndex++;
          if (this.currentBookIndex > this.books.length - 1) this.currentBookIndex = 0;
          this.bookService.currentBookIndex.next(this.currentBookIndex);
        },
        error: (error) => console.error('Error updating progress', error)
      });
    }
  }

  nextPart(nextPlayingPartId: number | null){
    const bookId = this.currentBook.id;
    const nextPartId = nextPlayingPartId as number;

    this.bookService.nextPart(bookId, nextPartId).subscribe((updatedPurchasedBook: PurchasedBook)=>{
      this.currentBook = updatedPurchasedBook;
      this.updateBookList();

      this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
      this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
      this.updateProgress();
      this.bookService.isPlaying.next(true);
      this.audioElement.nativeElement.play();
    })
  }

  getPlayPauseIcon(): string {
    if (this.currentBook.questionsActive) {
      return '../../../assets/icons/mic.svg';
    }
    return this.isPlaying ? '../../../assets/icons/pause.svg' : '../../../assets/icons/play_arrow.svg';
  }

  initializeSpeechRecognition() {
    if (!('webkitSpeechRecognition' in window)) {
      console.error('Web Speech API not supported in this browser.');
      return;
    }

    const SpeechRecognition = (window as any).webkitSpeechRecognition;
    this.recognition = new SpeechRecognition();
    this.recognition.continuous = true;
    this.recognition.interimResults = false;
    this.recognition.lang = 'en-US';

    this.recognition.onstart = () => {
      console.log('Speech recognition started');
    };

    this.recognition.onresult = (event: any) => {
      const transcript = event.results[event.resultIndex][0].transcript.trim().toLowerCase();
      console.log('Recognized: ', transcript);

      this.zone.run(() => {
        if (this.currentBook.questionsActive) {
          const matchedAnswer = this.currentBook.playingPart.answers.find(answer => transcript.includes(answer.text.toLowerCase()));

          if (matchedAnswer) {
            this.nextPart(matchedAnswer.nextPartId);
          }
        }
      });
    };

    this.recognition.onerror = (event: any) => {
      console.error('Speech recognition error: ', event.error);
    };

    this.recognition.onend = () => {
      console.log('Speech recognition ended');
      if (this.currentBook.questionsActive) {
        this.startRecognition(); // Restart recognition if still in question mode
      }
    };
  }

  startRecognition() {
    if (this.currentBook.questionsActive && this.recognition) {
      this.recognition.start();
      this.recognitionActive = true;
    }
  }

  stopRecognition() {
    if (this.recognition) {
      this.recognition.stop();
      this.recognitionActive = false;
    }
  }

  updatePlayingTime(event: MouseEvent): void {
    const progressWidth = this.progressArea.nativeElement.clientWidth;
    const clickedOffsetX = event.offsetX;
    const songDuration = this.audioElement.nativeElement.duration;

    this.audioElement.nativeElement.currentTime = (clickedOffsetX / progressWidth) * songDuration;
    
  }
}
