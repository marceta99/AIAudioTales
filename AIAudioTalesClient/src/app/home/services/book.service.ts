import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, finalize } from 'rxjs';
import { Basket, BasketItem, Book, Category, Purchase, PurchasedBook, SearchedBooks, Story } from 'src/app/entities';
import { environment } from 'src/environment/environment';
import { LoadingSpinnerService } from './loading-spinner.service';

@Injectable({
  providedIn: 'root'
})
export class BookService {

  private path = environment.apiUrl;
  libraryBooks = new Subject<SearchedBooks>();
  cartBooks = new BehaviorSubject<Book[]>([]);

  constructor(private httpClient: HttpClient, private spinnerService: LoadingSpinnerService) { }

  public getBooksFromCategory(bookCategory: number, page: number, pageSize: number) :Observable<Book[]>{
    const params = new HttpParams()
      .set('bookCategory', bookCategory)
      .set('page', page)
      .set('pageSize', pageSize);

    return this.httpClient.get<Book[]>(this.path + "Books/GetBooksFromCategory", {params});
  }
  public getBookWithId(bookId: number): Observable<Book>{
    this.spinnerService.setLoading(true);

    return this.httpClient.get<Book>
    (this.path + "Books/GetBook/"+bookId, {withCredentials: true}).pipe(
      finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }
  public purchaseBook(purchase: Purchase){
    return this.httpClient.post(this.path + "Books/PurchaseBook", purchase, {withCredentials: true, responseType: 'text'});
  }

  public userHasBook(bookId: number):Observable<boolean>{
    return this.httpClient.get<boolean>(this.path + "Books/UserHasBook/"+bookId, {withCredentials: true});
  }

  public isBasketItem(bookId: number):Observable<boolean>{
    return this.httpClient.get<boolean>(this.path + "Books/IsBasketItem/"+bookId, {withCredentials: true});
  }

  public getUserBooks():Observable<PurchasedBook[]>{
    this.spinnerService.setLoading(true);

    return this.httpClient.get<PurchasedBook[]>(this.path + "Books/GetUserBooks", {withCredentials: true}).pipe(
      finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }

  public getPurchasedBook(bookId: number):Observable<PurchasedBook>{
    this.spinnerService.setLoading(true);

    return this.httpClient.get<PurchasedBook>(this.path + "Books/GetPurchasedBook/"+ bookId, {withCredentials: true}).pipe(
      finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
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

  public addBasketItem(bookId: number){
    const params = new HttpParams().set('bookId', bookId);
    return this.httpClient.post<Book[]>(this.path+"Books/AddBasketItem", {},{ params, withCredentials : true});
  }

  public getBasket(): Observable<Basket> {
    return this.httpClient.get<Basket>(this.path+"Books/GetBasket", {withCredentials: true});
  }

  public removeBasketItem(itemId: number): Observable<Basket>{
    const params = new HttpParams().set('itemId', itemId);
    return this.httpClient.delete<Basket>(this.path+"Books/RemoveBasketItem",{ params, withCredentials : true});
  }
}

