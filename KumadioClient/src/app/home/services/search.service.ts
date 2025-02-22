import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import { ReturnBook } from "src/app/entities";
import { environment } from "src/environments/environment";

@Injectable()
export class SearchService {
  private baseUrl = environment.apiUrl;

  searchTerm: string = "";
    
  isSearchActive: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isSearchActive$: Observable<boolean> = this.isSearchActive.asObservable();
  searchHistory: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  searchHistory$: Observable<string[]> = this.searchHistory.asObservable();
  searchedBooks: Subject<ReturnBook[]> = new Subject<ReturnBook[]>();
  searchedBooks$: Observable<ReturnBook[]> = this.searchedBooks.asObservable();

  constructor(private httpClient: HttpClient) {}

  public getSearchHistory(): Observable<string[]> {
    return this.httpClient.get<string[]>(`${this.baseUrl}/library/search-history`, { withCredentials : true });
  }  

  public searchBooks(searchTerm: string, pageNumber: number, pageSize: number): Observable<ReturnBook[]> {
    const params = new HttpParams()
            .set('searchTerm', searchTerm)
            .set('pageNumber', pageNumber.toString())
            .set('pageSize', pageSize.toString());

    return this.httpClient.get<ReturnBook[]>(`${this.baseUrl}/catalog/search`, { params , withCredentials: true});
  }

  public saveSearchTerm(searchTerm: string){
    const params = new HttpParams().set('searchTerm', searchTerm);

    this.httpClient.post(`${this.baseUrl}/library/search-term`, {}, {params, withCredentials: true})
      .subscribe((res:any)=>console.log(res));
  }

}