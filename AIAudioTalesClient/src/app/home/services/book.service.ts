import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Book, Story } from 'src/app/entities';
import { environment } from 'src/environment/environment';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private path = environment.apiUrl;

  constructor(private httpClient: HttpClient) { }

  public getAllBooks():Observable<Book[]>{
    return this.httpClient.get<Book[]>(this.path + "Book/GetAllBooks", {withCredentials: true});
  }
  public getAllBooksWithImages():Observable<any>{
    return this.httpClient.get<Book[]>(this.path + "Books/GetAllBooks", {withCredentials: true});/// avoiding error
  }
  public getBookWithImage(bookId: number):Observable<any>{
    return this.httpClient.get<Book>(this.path + "Book/GetBookWithImage/"+bookId, {withCredentials: true});
  }
  public getBookStories(bookId: number): Observable<Story[]>{
    return this.httpClient.get<Story[]>
    (this.path + "Story/GetPlayableStoriesForBook/"+bookId, {withCredentials: true});
  }
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

}

