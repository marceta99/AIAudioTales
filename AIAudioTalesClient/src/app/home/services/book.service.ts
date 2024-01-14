import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Book, Purchase, PurchasedBook, Story } from 'src/app/entities';
import { environment } from 'src/environment/environment';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private path = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  public getBooksFromCategory(bookCategory: number, page: number, pageSize: number) :Observable<Book[]>{
    const params = new HttpParams()
      .set('bookCategory', bookCategory)
      .set('page', page)
      .set('pageSize', pageSize);

    return this.httpClient.get<Book[]>(this.path + "Books/GetBooksFromCategory", {params});
  }
  public getBookWithId(bookId: number): Observable<Book>{
    return this.httpClient.get<Book>
    (this.path + "Books/GetBook/"+bookId, {withCredentials: true});
  }
  public purchaseBook(purchase: Purchase){
    return this.httpClient.post(this.path + "Books/PurchaseBook", purchase, {withCredentials: true, responseType: 'text'});
  }

  public userHasBook(bookId: number):Observable<boolean>{
    return this.httpClient.get<boolean>(this.path + "Books/UserHasBook/"+bookId, {withCredentials: true});
  }

  public getUserBooks():Observable<PurchasedBook[]>{
    return this.httpClient.get<PurchasedBook[]>(this.path + "Books/GetUserBooks", {withCredentials: true});
  }

  public getPurchasedBook(bookId: number):Observable<PurchasedBook>{
    return this.httpClient.get<PurchasedBook>(this.path + "Books/GetPurchasedBook/"+ bookId, {withCredentials: true});
  }
}

