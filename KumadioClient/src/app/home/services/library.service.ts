import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, finalize, Observable, Subject } from "rxjs";
import { LoadingSpinnerService } from "src/app/common/services/loading-spinner.service";
import { Purchase, PurchasedBook, SearchedBooks } from "src/app/entities";
import { environment } from "src/environments/environment";

@Injectable()
export class LibraryService {
    private baseUrl = environment.apiUrl + '/library';

    public libraryBooks = new Subject<SearchedBooks>();
  
    purchasedBooks: BehaviorSubject<PurchasedBook[]> = new BehaviorSubject<PurchasedBook[]>([]);
  
    constructor(private httpClient: HttpClient, private spinnerService: LoadingSpinnerService) { }
  
    public userHasBook(bookId: number):Observable<boolean>{
      return this.httpClient.get<boolean>(`${this.baseUrl}/user-has-book/${bookId}`);
    }
  
    public getPurchasedBooks():Observable<PurchasedBook[]>{
      this.spinnerService.setLoading(true);
  
      return this.httpClient.get<PurchasedBook[]>(`${this.baseUrl}/purchased-books`).pipe(
        finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
      );
    }
  
    public getPurchasedBook(bookId: number):Observable<PurchasedBook>{
      this.spinnerService.setLoading(true);
  
      return this.httpClient.get<PurchasedBook>(`${this.baseUrl}/purchased-books/${bookId}`).pipe(
        finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
      );
    }
    
    public addToLibrary(bookId: number): Observable<void> {
      return this.httpClient.post<void>(`${this.baseUrl}/${bookId}`, {});
    }
}
