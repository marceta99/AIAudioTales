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
  currentBookIndex: number = 0;
  recognition: any;
  progress: number = 0;
  currentTime = '0:00';
  maxDuration = '0:00';
  isPlaying: boolean = false;
  recognitionActive: boolean = false;
  isTitleOverflowing: boolean = false;
  isArtistOverflowing: boolean = false;
  isFullScreen: boolean = false;

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
        this.setInitialCurrentBook();
      },
      error: error => {
          console.error('There was an error!', error);
      }
    });

    this.bookService.currentBookIndex.subscribe((index: number) => {
        if(this.currentBook) this.saveProgress(undefined, index); //do not save progress on initial load when currentBook is not yet loaded
    });

    this.bookService.isPlaying.subscribe((isPlaying: boolean)=> this.isPlaying = isPlaying);
  }

  ngOnDestroy(): void {
    this.stopRecognition();
  }

  setInitialCurrentBook(): void{
    for (let i = 0; i < this.books.length; i++) {
      if (this.books[i].isBookPlaying) {
        this.bookService.currentBookIndex.next(i);
        this.loadBook(i); // call loadBook() on initial load, because intialy saveProgress will not be called
        break; // Exit loop once the book is found
      }
    }
  }

  private loadBook(index: number): void {
    this.currentBook = this.books[index];
    this.currentBookIndex = index;
    this.cdr.detectChanges();

    this.isTitleOverflowing = this.isOverflowing(this.titleElement.nativeElement);
    this.isArtistOverflowing = this.isOverflowing(this.artistElement.nativeElement);

    this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
    this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
    this.progressBarUpdate();

    if (this.currentBook.questionsActive) {
      this.bookService.isPlaying.next(false);
      this.audioElement.nativeElement.pause();
      if(!this.recognitionActive) this.startRecognition(); //no need to start recognition again if it is started already
      this.progress = 100; //set progress bar width to 100% when questions are active, because that means that previous part is finished
    } else {
       if (this.isPlaying) this.audioElement.nativeElement.play();
       if (this.recognitionActive) this.stopRecognition(); // stop recognition if was active before
    }
  }

  private isOverflowing(element: HTMLElement): boolean {
    return element.scrollWidth > element.clientWidth;
  }

  public progressBarUpdate(): void {
    const current = this.audioElement.nativeElement.currentTime;
    const duration = this.audioElement.nativeElement.duration;
    if(!isNaN(duration)) {
      this.progress = (current / duration) * 100;
      this.currentTime = this.formatTime(current);
      this.maxDuration = this.formatTime(duration);
    }else {
      this.currentTime = "00:00"
      this.maxDuration = "00:00"
      this.progress = 0;
    }
  }

  private formatTime(seconds: number): string {
    const min = Math.floor(seconds / 60);
    const sec = Math.floor(seconds % 60);
    return `${min}:${sec < 10 ? '0' : ''}${sec}`;
  }

  public togglePlayPause(): void {
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

  public nextBook(): void {
    console.log("next ", this.currentBookIndex + 1 > this.books.length-1 ? 0 : this.currentBookIndex + 1)
    this.bookService.currentBookIndex.next(this.currentBookIndex + 1 > this.books.length-1 ? 0 : this.currentBookIndex + 1);
  }

  public prevBook(): void {
    if(this.currentBookIndex -1 < 0){
      const test = this.books.length - 1;
    }else{
      const ts = this.currentBookIndex - 1;
    }
    console.log("prev ", this.currentBookIndex - 1 < 0 ? this.books.length -1 : this.currentBookIndex - 1)
    this.bookService.currentBookIndex.next(this.currentBookIndex - 1 < 0 ? this.books.length -1 : this.currentBookIndex - 1);
  }

  private saveProgress(playingPosition?: number, nextBookIndex?: number, questionsActive?: boolean): void {
    const bookId = this.books[this.currentBookIndex].id;
    const currentTimeSec = playingPosition ? playingPosition : this.audioElement.nativeElement.currentTime;
    const nextBookId = nextBookIndex ? this.books[nextBookIndex].id : undefined ; 

    this.bookService.updateProgress(bookId, currentTimeSec, nextBookId, questionsActive).subscribe({
      next: (purchasedBook: PurchasedBook) => { 
        this.currentBook = purchasedBook;
        this.updateBookList();
        if(nextBookIndex !== undefined) this.loadBook(nextBookIndex); // only if there is need for next book, call loadbook() to load next book
      },
      error: (error) => console.error('Error updating progress', error)
    });
  }

  private updateBookList(): void {
    //update book list with changes of current book object
    this.books[this.currentBookIndex] = this.currentBook;
    this.bookService.purchasedBooks.next(this.books);
  }

  public activateQuestions(): void {
    if(this.currentBook.playingPart.answers.length > 0){
      this.bookService.isPlaying.next(false);

      this.bookService.activateQuestions(this.currentBook.id, this.audioElement.nativeElement.currentTime)
      .subscribe((purchasedBook: PurchasedBook) => {
        this.currentBook = purchasedBook;
        console.log("questions activated")
        this.updateBookList();
        this.startRecognition();
      })
    } else {
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

  public nextPart(nextPlayingPartId: number | null): void {
    const bookId = this.currentBook.id;
    const nextPartId = nextPlayingPartId as number;

    this.bookService.nextPart(bookId, nextPartId).subscribe((updatedPurchasedBook: PurchasedBook)=>{
      this.currentBook = updatedPurchasedBook;
      this.updateBookList();

      this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
      this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
      this.progressBarUpdate();
      this.bookService.isPlaying.next(true);
      this.audioElement.nativeElement.play();
    })
  }

  public getPlayPauseIcon(): string {
    if (this.currentBook.questionsActive) {
      return '../../../assets/icons/microphone-extra-small.svg';
    }
    return this.isPlaying ? '../../../assets/icons/pause.svg' : '../../../assets/icons/play_arrow.svg';
  }

  private initializeSpeechRecognition(): void {
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
        console.log("restart recognition")
        this.startRecognition(); // Restart recognition if still in question mode
      }
    };
  }

  private startRecognition(): void {
    if (this.currentBook.questionsActive && this.recognition) {
      this.recognition.start();
      this.recognitionActive = true;
    }
  }

  private stopRecognition(): void {
    if (this.recognition) {
      this.recognition.stop();
      this.recognitionActive = false;
    }
  }

  public updatePlayingTime(event: MouseEvent): void {
    const progressWidth = this.progressArea.nativeElement.clientWidth;
    const clickedOffsetX = event.offsetX;
    const songDuration = this.audioElement.nativeElement.duration;

    this.audioElement.nativeElement.currentTime = (clickedOffsetX / progressWidth) * songDuration;
    
  }

  public rewind(): void {
    const audio = this.audioElement.nativeElement;
    const rewindTime = 10; // time to rewind in seconds
    const playingPosition = Math.max(0, audio.currentTime - rewindTime); // set to 0 if it goes below 0
    audio.currentTime = playingPosition;
    this.progressBarUpdate(); // update the progress bar after rewinding

    if(this.currentBook.questionsActive) { // if questions(mic) are active, set questions acitve to false because now it is rewined 10sec and questions are not active
      this.saveProgress(playingPosition, undefined, false);
    }else {
      this.saveProgress(playingPosition, undefined, undefined);
    }
  }
}
