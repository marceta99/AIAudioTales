import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import { ReturnBook } from "src/app/entities";
import { environment } from "src/environments/environment";

@Injectable()
export class SearchService {
  private baseUrl = environment.apiUrl;

  private searchTermSubject: BehaviorSubject<string> = new BehaviorSubject("");
  public searchTerm$: Observable<string> = this.searchTermSubject.asObservable();

  public get searchTerm(): string {
   return this.searchTermSubject.value; 
  }

  public set searchTerm(term: string) {
    this.searchTermSubject.next(term);
  }
    
  isSearchActive: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isSearchActive$: Observable<boolean> = this.isSearchActive.asObservable();
  searchHistory: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  searchHistory$: Observable<string[]> = this.searchHistory.asObservable();
  searchedBooks: Subject<ReturnBook[]> = new Subject<ReturnBook[]>();
  searchedBooks$: Observable<ReturnBook[]> = this.searchedBooks.asObservable();

  constructor(private httpClient: HttpClient) {}

  public getSearchHistory(): Observable<string[]> {
    return this.httpClient.get<string[]>(`${this.baseUrl}/library/search-history`);
  }  

  public searchBooks(searchTerm: string, pageNumber: number, pageSize: number): Observable<ReturnBook[]> {
    const params = new HttpParams()
            .set('searchTerm', searchTerm)
            .set('pageNumber', pageNumber.toString())
            .set('pageSize', pageSize.toString());

    return this.httpClient.get<ReturnBook[]>(`${this.baseUrl}/catalog/search`, { params });
  }

  public saveSearchTerm(searchTerm: string){
    const params = new HttpParams().set('searchTerm', searchTerm);

    this.httpClient.post(`${this.baseUrl}/library/search-term`, {}, {params})
      .subscribe((res:any)=>console.log(res));
  }

}