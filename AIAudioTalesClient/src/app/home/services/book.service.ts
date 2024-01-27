import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, finalize } from 'rxjs';
import { Book, Category, Purchase, PurchasedBook, Story } from 'src/app/entities';
import { environment } from 'src/environment/environment';
import { LoadingSpinnerService } from './loading-spinner.service';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private path = environment.apiUrl;
  libraryBooks = new BehaviorSubject<Book[]>([]);

  constructor(private httpClient: HttpClient, private loadingSpinnerService: LoadingSpinnerService) { }

  public getBooksFromCategory(bookCategory: number, page: number, pageSize: number) :Observable<Book[]>{
    const params = new HttpParams()
      .set('bookCategory', bookCategory)
      .set('page', page)
      .set('pageSize', pageSize);

    return this.httpClient.get<Book[]>(this.path + "Books/GetBooksFromCategory", {params});
  }
  public getBookWithId(bookId: number): Observable<Book>{
    this.loadingSpinnerService.setLoading(true);

    return this.httpClient.get<Book>
    (this.path + "Books/GetBook/"+bookId, {withCredentials: true}).pipe(
      finalize(() => this.loadingSpinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }
  public purchaseBook(purchase: Purchase){
    return this.httpClient.post(this.path + "Books/PurchaseBook", purchase, {withCredentials: true, responseType: 'text'});
  }

  public userHasBook(bookId: number):Observable<boolean>{
    return this.httpClient.get<boolean>(this.path + "Books/UserHasBook/"+bookId, {withCredentials: true});
  }

  public getUserBooks():Observable<PurchasedBook[]>{
    this.loadingSpinnerService.setLoading(true);

    return this.httpClient.get<PurchasedBook[]>(this.path + "Books/GetUserBooks", {withCredentials: true}).pipe(
      finalize(() => this.loadingSpinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }

  public getPurchasedBook(bookId: number):Observable<PurchasedBook>{
    this.loadingSpinnerService.setLoading(true);

    return this.httpClient.get<PurchasedBook>(this.path + "Books/GetPurchasedBook/"+ bookId, {withCredentials: true}).pipe(
      finalize(() => this.loadingSpinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }

  public searchBooks(searchTerm: string, pageNumber: number, pageSize: number): Observable<Book[]> {
    const params = new HttpParams()
            .set('searchTerm', searchTerm)
            .set('pageNumber', pageNumber.toString())
            .set('pageSize', pageSize.toString());

    return this.httpClient.get<Book[]>(this.path+"Books/Search", { params, withCredentials : true});
  }

  public getSearchHistory(): Observable<string[]> {
    return this.httpClient.get<string[]>(this.path+"Books/GetSearchHistory", {withCredentials: true});
  }

  public saveSearchTerm(searchTerm: string){
    const params = new HttpParams().set('searchTerm', searchTerm);

    this.httpClient.post(this.path+"Books/SaveSearchTerm", searchTerm, {params, withCredentials: true}).subscribe((res:any)=>console.log(res));
  }

  public getAllCategories(): Observable<Category[]> {
    return this.httpClient.get<Category[]>(this.path+"Books/GetAllCategories", {withCredentials: true});
  }
}

