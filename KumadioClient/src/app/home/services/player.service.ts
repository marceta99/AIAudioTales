import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import { LoadingSpinnerService } from "src/app/common/services/loading-spinner.service";
import { PurchasedBook } from "src/app/entities";
import { environment } from "src/environments/environment";

@Injectable()
export class PlayerService {
  private baseUrl = environment.apiUrl + '/library';

  public remainingTime = new BehaviorSubject<number>(0); // Emits the remaining time in seconds
  public currentBookIndex: Subject<number> = new Subject<number>();
  public isPlaying: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public playerActive: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  public playerActive$: Observable<boolean> = this.playerActive.asObservable();

  constructor(private http: HttpClient, private spinnerService: LoadingSpinnerService) { }

  public processChildResponse(prompt: string): Observable<{ reply: string }> {
    return this.http.post<{ reply: string }>(`${this.baseUrl}/process-response`, { prompt });
  }

  public nextPart(bookId: number, nextPartId: number): Observable<PurchasedBook>{
    const nextPart = {
       bookId,
       nextPartId
    };

    return this.http.patch<PurchasedBook>(`${this.baseUrl}/next-part`, nextPart);
  }

  public activateQuestions(bookId: number, playingPosition: number): Observable<PurchasedBook> {
    const activateQuestions = {
      bookId,
      playingPosition
    };

    return this.http.patch<PurchasedBook>(`${this.baseUrl}/activate-questions`, activateQuestions);
  }

  public updateProgress(bookId: number, playingPosition?: number, nextBookId?: number, questionsActive?: boolean): Observable<PurchasedBook> {
    const progress: any = { bookId };

    // Conditionally add optional properties if they have values
    if (playingPosition !== undefined) {
      progress.playingPosition = playingPosition;
    }
    
    if (nextBookId !== undefined) {
      progress.nextBookId = nextBookId;
    }

    if(questionsActive !== undefined) {
      progress.questionsActive = questionsActive;
    }

    return this.http.patch<PurchasedBook>(`${this.baseUrl}/update-progress`, progress);
  }

  public startBookAgain(bookId: number): Observable<PurchasedBook> {
    return this.http.patch<PurchasedBook>(`${this.baseUrl}/restart-book/${bookId}`, {});
  }
}