import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import { ReturnBook } from "src/app/entities";
import { environment } from "src/environment/environment";

@Injectable({
    providedIn: 'root'
})
export class SearchService {
  private path = environment.apiUrl;

  searchTerm: string = "";
    
  isSearchActive: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isSearchActive$: Observable<boolean> = this.isSearchActive.asObservable();
  searchHistory: BehaviorSubject<string[]> = new BehaviorSubject<string[]>([]);
  searchHistory$: Observable<string[]> = this.searchHistory.asObservable();
  searchedBooks: Subject<ReturnBook[]> = new Subject<ReturnBook[]>();
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

  public saveSearchTerm(searchTerm: string){
    const params = new HttpParams().set('searchTerm', searchTerm);

    this.httpClient.post(this.path+"Books/SaveSearchTerm", searchTerm, {params}).subscribe((res:any)=>console.log(res));
  }

}