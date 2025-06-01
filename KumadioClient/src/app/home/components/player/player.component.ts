import { ChangeDetectorRef, Component, ElementRef, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { PurchasedBook } from 'src/app/entities';
import { PlayerService } from '../../services/player.service';
import { CommonModule } from '@angular/common';
import { LibraryService } from '../../services/library.service';

@Component({
  selector: 'app-player',
  templateUrl: 'player.component.html',
  styleUrls: ['player.component.scss'],
  imports: [CommonModule],
  providers: []
})
export class PlayerComponent implements OnInit, OnDestroy {
  private books!: PurchasedBook[];
  private currentBookIndex: number = 0;
  private recognition: any;
  private recognitionActive = false;   // “am I actually running right now?”
  private shouldListen = false;     // “do I want to be running?”
  private transcriptBuffer = '';
  private transcriptTimeout: any;
  private failedAttempts = 0;
  private readonly maxFailedAttempts = 3;
  private lastTranscriptFragment = '';
  private repeatAudio: HTMLAudioElement | null = null;

  public progress: number = 0;
  public currentTime = '0:00';
  public maxDuration = '0:00';
  public isPlaying: boolean = false;
  public isTitleOverflowing: boolean = false;
  public isArtistOverflowing: boolean = false;
  public isFullScreen: boolean = false;
  public isLoading: boolean = false;
  public currentBook!: PurchasedBook;
  public isProcessing = false;

  @ViewChild('audioElement', { static: false }) audioElement!: ElementRef;  
  @ViewChild('progressArea', { static: false }) progressArea!: ElementRef;
  @ViewChild('titleElement') titleElement!: ElementRef;
  @ViewChild('artistElement') artistElement!: ElementRef;

  constructor(
    private libraryService: LibraryService,
    private playerService: PlayerService,
    private cdr: ChangeDetectorRef,
    private zone: NgZone) {}

  ngOnInit(): void {
    this.initializeSpeechRecognition();

    this.libraryService.getPurchasedBooks().subscribe({
      next: (books : PurchasedBook[] ) => {
        console.log(books);
        this.books = books;  
        this.setInitialCurrentBook();
      },
      error: error => {
          console.error('There was an error!', error);
      }
    });

    this.libraryService.purchasedBooks.subscribe((books: PurchasedBook[])=>{
      console.log("purchased books observable")
      this.books = books;
    })

    this.playerService.currentBookIndex.subscribe((index: number) => {
        if(this.currentBook) this.saveProgress(undefined, index); //do not save progress on initial load when currentBook is not yet loaded
        else this.loadBook(index)
    });

    this.playerService.isPlaying.subscribe((isPlaying: boolean)=> this.isPlaying = isPlaying);
  }

  ngOnDestroy(): void {
    this.stopRecognition();
  }

  setInitialCurrentBook(): void{
    for (let i = 0; i < this.books.length; i++) {
      if (this.books[i].isBookPlaying) {
        this.playerService.currentBookIndex.next(i);
        this.loadBook(i); // call loadBook() on initial load, because intialy saveProgress will not be called
        break; // Exit loop once the book is found
      }
    }
  }

  private loadBook(index: number): void {
    if (!this.currentBook) {
      console.log("player active")
      this.playerService.playerActive.next(true); // set playerActive to true if previously there was not activePlayer
    }

    this.currentBook = this.books[index];
    this.currentBookIndex = index;
    this.cdr.detectChanges();

    this.isTitleOverflowing = this.isOverflowing(this.titleElement.nativeElement);
    this.isArtistOverflowing = this.isOverflowing(this.artistElement.nativeElement);

    this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
    this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
    this.progressBarUpdate();

    if (this.currentBook.questionsActive) {
      this.playerService.isPlaying.next(false);
      this.audioElement.nativeElement.pause();
      if(!this.recognitionActive) this.startRecognition(); //no need to start recognition again if it is started already
      this.progress = 100; //set progress bar width to 100% when questions are active, because that means that previous part is finished
    } else {
       if (this.isPlaying) this.audioElement.nativeElement.play();
       if (this.recognitionActive) this.stopRecognition(); // stop recognition if was active before
    }

    this.isLoading = false;
  }

  private isOverflowing(element: HTMLElement): boolean {
    return element.scrollWidth > element.clientWidth;
  }

  public progressBarUpdate(): void {
    const current = this.audioElement.nativeElement.currentTime;
    const duration = this.audioElement.nativeElement.duration;
  
    if (!isNaN(duration)) {
      this.progress = (current / duration) * 100;
      this.currentTime = this.formatTime(current);
      this.maxDuration = this.formatTime(duration);
  
      const remainingTime = duration - current;
      this.playerService.remainingTime.next(Math.max(0, Math.floor(remainingTime))); // Emit remaining time
    } else {
      this.currentTime = '00:00';
      this.maxDuration = '00:00';
      this.progress = 0;
  
      this.playerService.remainingTime.next(0); // Emit 0 if duration is not valid
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
      this.playerService.isPlaying.next(!this.isPlaying);
    }
  }

  public nextBook(): void {
    this.isLoading = true;
    this.playerService.currentBookIndex.next(this.currentBookIndex + 1 > this.books.length-1 ? 0 : this.currentBookIndex + 1);
  }

  public prevBook(): void {
    this.isLoading = true;
    this.playerService.currentBookIndex.next(this.currentBookIndex - 1 < 0 ? this.books.length -1 : this.currentBookIndex - 1);
  }

  private saveProgress(playingPosition?: number, nextBookIndex?: number, questionsActive?: boolean): void {
    const bookId = this.books[this.currentBookIndex].bookId;
    const currentTimeSec = playingPosition ? playingPosition : this.audioElement.nativeElement.currentTime;
    const nextBookId = nextBookIndex ? this.books[nextBookIndex].bookId : undefined ; 

    this.playerService.updateProgress(bookId, currentTimeSec, nextBookId, questionsActive).subscribe({
      next: (purchasedBook: PurchasedBook) => { 
        console.log("progress updated")
        this.currentBook = purchasedBook;
        this.updateBookList();
        if(nextBookIndex !== undefined) this.loadBook(nextBookIndex); // only if there is need for next book, call loadbook() to load next book
      },
      error: (error) => console.error('Error updating progress', error)
    });
  }

  private updateBookList(): void {
    // Preserve the remainingTime for the currently playing book
    if (this.books[this.currentBookIndex]) {
      const remainingTime = this.books[this.currentBookIndex].remainingTime;
      this.books[this.currentBookIndex] = { ...this.currentBook, remainingTime };
    } else {
      this.books[this.currentBookIndex] = this.currentBook;
    }
  
    // Update the purchasedBooks observable
    this.libraryService.purchasedBooks.next(this.books);
  }

  public activateQuestions(): void {
    if(this.currentBook.playingPart.answers.length > 0){
      this.playerService.isPlaying.next(false);

      this.playerService.activateQuestions(this.currentBook.bookId, this.audioElement.nativeElement.currentTime)
      .subscribe((purchasedBook: PurchasedBook) => {
        this.currentBook = purchasedBook;
        console.log("questions activated")
        this.updateBookList();
        this.startRecognition();
      })
    } else {
      //if there is no more answers it means that user have reached the end of that book part and we should
      // set current book back to start and play the next book
      this.playerService.startBookAgain(this.currentBook.bookId).subscribe({
        next: (updatedPurchasedBook: PurchasedBook) => {
          console.log("start again response ", updatedPurchasedBook)
          this.currentBook = updatedPurchasedBook;
          this.updateBookList();

          //load next book when this is finished
          this.currentBookIndex++;
          if (this.currentBookIndex > this.books.length - 1) this.currentBookIndex = 0;
          this.playerService.currentBookIndex.next(this.currentBookIndex);
        },
        error: (error) => console.error('Error updating progress', error)
      });
    }
  }

  public nextPart(nextPlayingPartId: number | null): void {
    const bookId = this.currentBook.bookId;
    const nextPartId = nextPlayingPartId as number;

    this.playerService.nextPart(bookId, nextPartId).subscribe((updatedPurchasedBook: PurchasedBook)=>{
      this.currentBook = updatedPurchasedBook;
      this.updateBookList();

      this.audioElement.nativeElement.src = this.currentBook.playingPart.partAudioLink;
      this.audioElement.nativeElement.currentTime = this.currentBook.playingPosition;
      this.progressBarUpdate();
      this.playerService.isPlaying.next(true);
      this.audioElement.nativeElement.play();
    })
  }

  public getPlayPauseIcon(): string {
    if (this.currentBook.questionsActive) {
      return '../../../assets/icons/mic_24_white.svg';
    }
    return this.isPlaying ? '../../../assets/icons/pause_circle.svg' : '../../../assets/icons/play_circle.svg';
  }

  /*private initializeSpeechRecognition(): void {
    const SpeechRecognition = (window as any).SpeechRecognition
                            || (window as any).webkitSpeechRecognition;
    if (!SpeechRecognition) {
      console.error('Web Speech API not supported');
      return;
    }

    this.recognition = new SpeechRecognition();
    this.recognition.continuous      = true;
    this.recognition.interimResults  = true;
    this.recognition.maxAlternatives = 1;
    this.recognition.lang            = 'sr-RS'    //'sr-RS' en-US;

    this.recognition.onstart = () => {
      console.log('🎙️ Speech recognition actually started');
      this.recognitionActive = true;    // mark it running
    };

    this.recognition.onresult = (event: any) => {
      const transcript = event.results[event.resultIndex][0].transcript.trim().toLowerCase();
      console.log("result ", transcript);

      if (!this.isTranscriptValid(transcript)) return;

      // IGNORIŠI ako je isto kao prethodni deo → sprečava dupliranje
      if (transcript === this.lastTranscriptFragment) return;

      this.lastTranscriptFragment = transcript;

      this.zone.run(() => {
        clearTimeout(this.transcriptTimeout);

        this.transcriptBuffer = transcript;

        this.transcriptTimeout = setTimeout(() => {
          const finalTranscript = this.transcriptBuffer.trim();
          this.transcriptBuffer = '';
          this.lastTranscriptFragment = '';

          this.processChildResponse(finalTranscript);
        }, 1200);
  });
    };

    this.recognition.onerror = (event: any) => {
      // handle the no‑speech auto‑retry
      if (event.error === 'no-speech' && this.shouldListen) {
        console.warn('No speech—but I’ll retry in 300ms');
        setTimeout(() => this.recognition.start(), 300);
        return;
      }
    
      // ignore the user‑initiated abort
      if (event.error === 'aborted') {
        // no-op
        return;
      }
    
      // anything else is actually bad
      console.error('Speech error:', event.error);
    };
    
    this.recognition.onend = () => {
      this.recognitionActive = false;
      console.log('🎙️ Speech ended');
      // remove your unconditional restart from here!
      // if you really want to restart after results, do it in onresult instead.
    };
  }*/

  private initializeSpeechRecognition(): void {
    const SpeechRecognition = (window as any).SpeechRecognition
                            || (window as any).webkitSpeechRecognition;
    if (!SpeechRecognition) {
      console.error('Web Speech API not supported');
      return;
    }

    this.recognition = new SpeechRecognition();
    this.recognition.continuous      = true;   // pokušava da sluša više segmenata govora
    this.recognition.interimResults  = true;   // ne prikazujemo nepotpune rezultate
    this.recognition.maxAlternatives = 1;
    this.recognition.lang            = 'sr-RS';

    // Kada se prepoznavanje zapravo pokrene
    this.recognition.onstart = () => {
      console.log('🎙️ Speech recognition actually started');
      this.recognitionActive = true; // markiramo da je aktivno
    };

    // Kada prepozna fragment govora (rezultat)
    this.recognition.onresult = (event: any) => {
      const transcript = event.results[event.resultIndex][0]
                          .transcript.trim().toLowerCase();
      console.log("result ", transcript);

      if (!this.isTranscriptValid(transcript)) return;

      // Ignorišemo dupliranje istog fragmenta
      if (transcript === this.lastTranscriptFragment) return;
      this.lastTranscriptFragment = transcript;

      this.zone.run(() => {
        clearTimeout(this.transcriptTimeout);
        this.transcriptBuffer = transcript;

        // Čekamo da dete prestane govoriti (1.2 s pauze) pre nego što procesiramo
        this.transcriptTimeout = setTimeout(() => {
          const finalTranscript = this.transcriptBuffer.trim();
          this.transcriptBuffer = '';
          this.lastTranscriptFragment = '';

          this.processChildResponse(finalTranscript);
        }, 1200);
      });
    };

    // Greške u prepoznavanju
    this.recognition.onerror = (event: any) => {
      // Ako se ništa ne čuje i treba da slušamo, restartujemo nakon 300 ms
      if (event.error === 'no-speech' && this.shouldListen) {
        console.warn('No speech—but I’ll retry in 300ms');
        setTimeout(() => {
          if (this.shouldListen && this.currentBook?.questionsActive) {
            this.recognition.start();
          }
        }, 300);
        return;
      }
      // Ignorišemo abort koji smo mi sami pozvali
      if (event.error === 'aborted') {
        return;
      }
      console.error('Speech error:', event.error);
    };
    
    // Kada prepoznavanje stane (najčešće zbog tišine ili abort)
    this.recognition.onend = () => {
      this.recognitionActive = false;
      console.log('🎙️ Speech ended');
      // Restartujemo PREPOZNAVANJE SAMO ako:
      // 1) želimo da slušamo (shouldListen)
      // 2) trenutno smo u delu sa pitanjima (questionsActive)
      // 3) nismo u toku obrade backenda (isProcessing === false)
      if (this.shouldListen && this.currentBook?.questionsActive && !this.isProcessing) {
        setTimeout(() => {
          if (this.shouldListen && !this.recognitionActive && !this.isProcessing) {
            this.recognition.start();
          }
        }, 200);
      }
    };

    // Ako prepozna glas korisnika, pauziramo audio (knjige) da ne bi ometalo
    this.recognition.onspeechstart = () => {
      console.log('🎤 User started speaking—pausing audio');
      // Ako se u tom trenutku reproducira audio knjige (questionsActive=false),
      // nemamo šta da slušamo. U svakom slučaju pauziramo.
      if (this.audioElement?.nativeElement) {
        this.audioElement.nativeElement.pause();
      }
    };
}
  
  private startRecognition(): void {
    // “I want the mic open”
    this.shouldListen = true;
  
    // only call start() if we’re not already running
    if (this.recognition && !this.recognitionActive) {
      try {
        // immediately mark running so subsequent calls are skipped
        this.recognitionActive = true;
        this.recognition.start();
      } catch (err: any) {
        // swallow the “already started” error
        if (err.name !== 'InvalidStateError') {
          console.error('Unexpected speech‑start error:', err);
        }
      }
    }
  }
    
  private stopRecognition(): void {
    this.shouldListen = false;
    if (this.recognition && this.recognitionActive) {
      // abort() will silently end without firing an “aborted” error
      this.recognition.abort();
      // onend will still fire, where you set recognitionActive = false
    }
  }
    
  private isTranscriptValid(transcript: string): boolean {
    // Filter out empty or irrelevant transcripts
    if (!transcript || transcript.length < 2) {
      // Ignore very short or empty transcripts
      return false;
    }

    // Optionally, implement more advanced filtering
    // For example, check if transcript contains at least one valid word character
    const validWordPattern = /[a-zA-Zčćžđš]/; // Adjust pattern for your language
    if (!validWordPattern.test(transcript)) {
      return false;
    }

    return true;
  }

  private processChildResponse(transcript: string): void {
    const matchedAnswer = this.currentBook.playingPart.answers.find(answer =>
      transcript.includes(answer.text.toLowerCase())
    );

    if (matchedAnswer) {
      this.failedAttempts = 0; // resetuj pokušaje
      this.nextPart(matchedAnswer.nextPartId);
    } else if (this.failedAttempts < this.maxFailedAttempts) {
      this.sendToBackendForProcessing(transcript);
    } else {
      this.promptChildToRepeat(); // ne šaljemo više requestova
    }
  }

  private sendToBackendForProcessing(transcript: string): void {
    const possibleAnswers = this.currentBook.playingPart.answers.map(answer => answer.text);
    const prompt = `
      You are an assistant helping to interpret a child's response in an interactive audiobook. The child was asked a question with the following possible answers: ${possibleAnswers.join(', ')}.

      Given the child's response: "${transcript}"

      Determine which of the possible answers the child intended. If the response is unclear or doesn't match any options, reply "unclear".

      Respond with only the chosen answer or "unclear".
    `;

    this.playerService.processChildResponse(prompt).subscribe({
      next: (response: { reply: string }) => {
        const chosenAnswer = response.reply;
        if (chosenAnswer && chosenAnswer.toLowerCase() !== 'unclear') {
          const matchedAnswer = this.currentBook.playingPart.answers.find(answer =>
            answer.text.toLowerCase() === chosenAnswer.toLowerCase()
          );
          if (matchedAnswer) {
            this.failedAttempts = 0;
            this.nextPart(matchedAnswer.nextPartId);
            return;
          }
        }
        
        // 3a) Backend je odgovorio – vraćamo mikrofon u akciju
        this.isProcessing = false;
        this.startRecognition();  // startuje recognizer samo ako questionsActive=true

        this.failedAttempts++;
        this.promptChildToRepeat();
      },
      error: error => {
        // 3a) Backend je odgovorio – vraćamo mikrofon u akciju
        this.isProcessing = false;
        this.startRecognition();  // startuje recognizer samo ako questionsActive=true

        console.error('There was an error!', error);
        this.failedAttempts++;
        this.promptChildToRepeat();
      }
    });
  }

  private promptChildToRepeat(): void {
    // Ako već postoji instanca „repeat“ zvuka (iz prethodnih pokušaja), prekidamo je
    if (this.repeatAudio) {
      this.repeatAudio.pause();
      this.repeatAudio = null;
    }

    // Kreiramo novu instancu
    this.repeatAudio = new Audio('assets/audio/repeat_audio.mp3');

    // Kad svira „repeat“, NEMOJ da pozivaš stopRecognition()
    // jer želiš da mikrofon ostane aktivan i spreman da uhvati neki fragment govora.
    this.repeatAudio.play();

    // Kada se „repeat“ završi prirodno, oslobodi instancu
    this.repeatAudio.onended = () => {
      this.repeatAudio = null;
    };
  }


  public updatePlayingTime(event: MouseEvent): void {
    const progressWidth = this.progressArea.nativeElement.clientWidth;
    const clickedOffsetX = event.offsetX;
    const duration = this.audioElement.nativeElement.duration;

    const playingPosition = (clickedOffsetX / progressWidth) * duration; 
    this.audioElement.nativeElement.currentTime  = playingPosition;

    if(playingPosition < duration){
      if (this.currentBook.questionsActive) { // if questions(mic) are active, set questions active to false because now it is rewound 10sec and questions are not active
        this.saveProgress(playingPosition, undefined, false);
      } else {
        this.saveProgress(playingPosition, undefined, undefined);
      }
    }
  }

  public rewind(): void {
    const audio = this.audioElement.nativeElement;
    const rewindTime = 10; // time to rewind in seconds
    const playingPosition = Math.max(0, audio.currentTime - rewindTime); // set to 0 if it goes below 0
    audio.currentTime = playingPosition;
    this.progressBarUpdate(); // update the progress bar after rewinding

    if (this.currentBook.questionsActive) { // if questions(mic) are active, set questions active to false because now it is rewound 10sec and questions are not active
      this.saveProgress(playingPosition, undefined, false);
    } else {
      this.saveProgress(playingPosition, undefined, undefined);
    }
  }

  public fastForward(): void {
    const audio = this.audioElement.nativeElement;
    const forwardTime = 10; // 10 seconds forward
    const playingPosition = Math.min(audio.duration, audio.currentTime + forwardTime);
    audio.currentTime = playingPosition;
    this.progressBarUpdate();
  }
  
  public toggleFullScreen(): void {
    this.isFullScreen = !this.isFullScreen;
  }
}
