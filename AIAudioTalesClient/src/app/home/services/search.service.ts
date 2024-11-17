import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable } from "rxjs";
import { ReturnBook } from "src/app/entities";
import { environment } from "src/environment/environment";

@Injectable({
    providedIn: 'root'
})
export class SearchService {
  private path = environment.apiUrl;
    
  searchHistory: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  searchHistory$: Observable<string[]> = this.searchHistory.asObservable();
  searchedBooks: BehaviorSubject<ReturnBook[]> = new BehaviorSubject<ReturnBook[]>([]);
  searchedBooks$: Observable<ReturnBook[]> = this.searchedBooks.asObservable();

  constructor(private httpClient: HttpClient) {}

  public getSearchHistory(): Observable<string[]> {
    return this.httpClient.get<string[]>(this.path+"Books/GetSearchHistory");
  }  

  public searchBooks(searchTerm: string, pageNumber: number, pageSize: number): Observable<ReturnBook[]> {
    const params = new HttpParams()
            .set('searchTerm', searchTerm)
            .set('pageNumber', pageNumber.toString())
            .set('pageSize', pageSize.toString());

    return this.httpClient.get<ReturnBook[]>(this.path+"Books/Search", { params , withCredentials: true});
  }

}