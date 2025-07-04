import { ChangeDetectorRef, Component, ElementRef, NgZone, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { PurchasedBook, userResponseDto } from 'src/app/entities';
import { PlayerService } from '../../services/player.service';
import { CommonModule } from '@angular/common';
import { LibraryService } from '../../services/library.service';
import { isNativeApp } from 'src/utils/functions';
import {
  SpeechRecognition,
  type PermissionStatus   // ← plugin’s interface: { speechRecognition: PermissionState }
} from '@capacitor-community/speech-recognition';
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
     if (!isNativeApp()) this.initializeWebSpeechRecognition();

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

    this.playerService.nextPart(bookId, nextPartId).subscribe((updatedPurchasedBook: PurchasedBook) => {
      this.currentBook = updatedPurchasedBook;
      this.updateBookList();

      const audio: HTMLAudioElement = this.audioElement.nativeElement;
      audio.src = this.currentBook.playingPart.partAudioLink;
      audio.currentTime = this.currentBook.playingPosition;
      this.progressBarUpdate();

      // Wait for enough data to begin playback
      const onCanPlay = () => {
        audio.removeEventListener('canplay', onCanPlay);
        audio.play().catch(err => {
          console.warn('Play interrupted or prevented:', err);  
        });
        this.playerService.isPlaying.next(true);
      };

      audio.addEventListener('canplay', onCanPlay);
      // trigger load() explicitly
      audio.load();

      // Stop any recognition that might have still been running
      if (this.recognitionActive) {
        this.stopRecognition();
      }
    });
  }

  private async startRecognition() {
    // Mark that we want to listen
    this.shouldListen = true;
    
    if (isNativeApp()) {
          console.log("start mobile")
          await this.startNativeSpeechLoop();
    } else {
        // Only call start() if it's not already running
      if (this.recognition && !this.recognitionActive) {
        try {
          // Immediately set recognitionActive so we don’t double‐enter before onstart fires
          this.recognitionActive = true;
          console.log("start web");
          this.recognition.start();
            
        } catch (err: any) {
          // Swallow the "already started" error if it somehow races
          if (err.name !== 'InvalidStateError') {
            console.error('Unexpected speech‐start error:', err);
          }
          // Reset the flag so our next onend–>start flow can work
          this.recognitionActive = false;
        }
      }
    }
  }

  private stopRecognition(): void {
    this.shouldListen = false;
    isNativeApp() ? this.stopNativeSpeechLoop(): this.stopWebRecognition();
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

  //#region WEB Speech Recognition
  private initializeWebSpeechRecognition(): void {
    // pick whichever Web Speech API constructor is available
    const SpeechRecognition = (window as any).SpeechRecognition
                            || (window as any).webkitSpeechRecognition;
    if (!SpeechRecognition) {
      console.error('Web Speech API not supported');
      return;
    }

    this.recognition = new SpeechRecognition();
    this.recognition.continuous      = true;   // attempt to capture multiple segments
    this.recognition.interimResults  = true;   // we’ll ignore “interim” in onresult if not final
    this.recognition.maxAlternatives = 1;
    this.recognition.lang            = 'sr-RS';

    // When the engine actually starts listening
    this.recognition.onstart = () => {
      console.log('🎙️ Speech recognition actually started');
      this.recognitionActive = true;
    };

    // When we get any result (interim or final)
    this.recognition.onresult = (event: any) => {
      // Loop over every new result from event.resultIndex onward
      for (let i = event.resultIndex; i < event.results.length; i++) {
        const result = event.results[i];
        const transcriptSegment = result[0].transcript.trim().toLowerCase();

        if (!this.isTranscriptValid(transcriptSegment)) {
          // skip garbage / very short segments
          continue;
        }

        if (result.isFinal) {
          // Final transcript arrived
          console.log('🟢 Final transcript:', transcriptSegment);

          // Clear any buffers
          this.lastTranscriptFragment = '';
          this.transcriptBuffer = '';

          // Process inside Angular zone so UI updates properly
          this.zone.run(() => {
            this.processChildResponse(transcriptSegment);
          });
        } else {
          // Interim transcript: just store buffer, but do NOT re‐fire logic
          this.transcriptBuffer = transcriptSegment;
          console.log('⚫ Interim transcript:', transcriptSegment);
        }
      }
    };

    // Handle errors
    this.recognition.onerror = (event: any) => {
      // If we got "no‐speech" while we still want to listen, schedule a restart
      if (event.error === 'no-speech' && this.shouldListen && this.currentBook?.questionsActive && !this.isProcessing) {
        console.warn('No speech—but I’ll retry in 300ms');
        setTimeout(() => {
          // Only start again if we truly still want to listen
          if (this.shouldListen && this.currentBook?.questionsActive && !this.recognitionActive && !this.isProcessing) {
            this.startRecognition();
          }
        }, 300);
        return;
      }

      // If user intentionally aborted, ignore
      if (event.error === 'aborted') {
        return;
      }

      console.error('Speech error:', event.error);
    };

    // When the recognition session ends (e.g. silence, or we explicitly called abort())
    this.recognition.onend = () => {
      this.recognitionActive = false;
      console.log('🎙️ Speech ended');

      // Only try to restart if:
      // 1) we still want to listen (shouldListen === true)
      // 2) we're in the questions phase (currentBook.questionsActive === true)
      // 3) we are not currently waiting on a backend call (isProcessing === false)
      if (this.shouldListen && this.currentBook?.questionsActive && !this.isProcessing) {
        // Use our helper so it checks recognitionActive internally
        this.startRecognition();
      }
    };

    // As soon as the user starts speaking, we pause the audiobook so voices don’t overlap
    this.recognition.onspeechstart = () => {
      console.log('🎤 User started speaking—pausing audio');
      if (this.audioElement?.nativeElement) {
        this.audioElement.nativeElement.pause();
      }
    };
  }

  private stopWebRecognition(): void {
    if (this.recognition && this.recognitionActive) {
      // Abort will trigger onend, which sets recognitionActive=false
      this.recognition.abort();
    }
  }
  //#endregion

  //#region Android Speech Recognition
  public async startNativeSpeechLoop() {
    // feature check
    const { available } = await SpeechRecognition.available();
    if (!available) {
      console.warn('Native speech recognition unavailable');
      return;
    }

    // permission check
    let perms = await SpeechRecognition.checkPermissions();
    if (perms.speechRecognition !== 'granted') {
      perms = await SpeechRecognition.requestPermissions();
      if (perms.speechRecognition !== 'granted') {
        alert(
          'Please enable Microphone & Speech permissions in Settings to use voice answers.'
        );
        return;
      }
    }

    // loop until we have a usable utterance
    this.shouldListen = true;
    while (this.shouldListen) {
      try {
        // return as soon as you speak or timeout
        const { matches } = await SpeechRecognition.start({
          popup: false,
          language: 'sr-RS',
          partialResults: false,
          maxResults: 1
        });

        const utterance = matches?.[0]?.trim(); 
        if (utterance) {
          // we got speech — stop looping and hand off
          this.shouldListen = false;
          this.zone.run(() => this.processChildResponse(utterance.toLowerCase()));
        } else {
          // empty string: retry the loop
          console.debug('Native speech: no content, retrying…');
        }

      } catch (err: any) {
        console.warn('Native speech error:', err.message);
        // if it’s one of the “no match” cases, we just retry
        if (
          err.message === 'No match' ||
          err.message === 'Client side error' 
        ) {
          continue;
        } else { 
          // some other fatal error — bail
          console.error('Fatal native speech error', err);
          this.shouldListen = false;
        }
      }
    }
  }

  public async stopNativeSpeechLoop() {
    this.shouldListen = false;
    try {
      await SpeechRecognition.stop();
      await SpeechRecognition.removeAllListeners();
    } catch {
      /* ignore */
    }
  }
  //#endregion 

  //#region Book List Logic
  public nextBook(): void {
    this.isLoading = true;
    this.playerService.currentBookIndex.next(this.currentBookIndex + 1 > this.books.length-1 ? 0 : this.currentBookIndex + 1);
    
    if (this.recognitionActive) {
        this.stopRecognition();
    }
  }

  public prevBook(): void {
    this.isLoading = true;
    this.playerService.currentBookIndex.next(this.currentBookIndex - 1 < 0 ? this.books.length -1 : this.currentBookIndex - 1);

    if (this.recognitionActive) {
        this.stopRecognition();
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

  private setInitialCurrentBook(): void {
    for (let i = 0; i < this.books.length; i++) {
      if (this.books[i].isBookPlaying) {
        this.playerService.currentBookIndex.next(i);
        this.loadBook(i); // call loadBook() on initial load, because intialy saveProgress will not be called
        break; // Exit loop once the book is found
      }
    }
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
  //#endregion

  //#region Player Actions
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

  public toggleFullScreen(): void {
    this.isFullScreen = !this.isFullScreen;
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

    if (this.recognitionActive) {
        this.stopRecognition();
    }
  }

  public fastForward(): void {
    const audio = this.audioElement.nativeElement;
    const forwardTime = 10; // 10 seconds forward
    const playingPosition = Math.min(audio.duration, audio.currentTime + forwardTime);
    audio.currentTime = playingPosition;
    this.progressBarUpdate();

    if (this.recognitionActive) {
        this.stopRecognition();
    }
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

  private formatTime(seconds: number): string {
    const min = Math.floor(seconds / 60);
    const sec = Math.floor(seconds % 60);
    return `${min}:${sec < 10 ? '0' : ''}${sec}`;
  }

  private isOverflowing(element: HTMLElement): boolean {
    return element.scrollWidth > element.clientWidth;
  }
  //#endregion

  //#region Response Processing
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
      this.promptChildToRepeat();
    }
  }

  private sendToBackendForProcessing(transcript: string): void {
    this.isProcessing = true;
    const dto: userResponseDto = {
      partId: this.currentBook.playingPart.id,  
      transcript: transcript,
      possibleAnswers: this.currentBook.playingPart.answers.map(a => a.text)
    };

    this.playerService.processChildResponse(dto).subscribe({
      next: (response: { reply: string }) => {
        const chosenAnswer = response.reply;
        if (chosenAnswer && chosenAnswer.toLowerCase() !== 'unclear') {
          const matchedAnswer = this.currentBook.playingPart.answers.find(answer =>
            answer.text.toLowerCase().includes(chosenAnswer.toLowerCase())
          );
          if (matchedAnswer) {
            this.failedAttempts = 0;
            this.nextPart(matchedAnswer.nextPartId);
            return;
          }
        }
        
        // Backend responded, turn on mic again
        this.isProcessing = false;
        this.startRecognition();

        this.failedAttempts++;
        this.promptChildToRepeat();
      },
      error: error => {
        console.error('There was an error!', error);

        // Backend responded, turn on mic again
        this.isProcessing = false;
        this.startRecognition();

        this.failedAttempts++;
        this.promptChildToRepeat();
      }
    });
  }

  //#endregion
}
