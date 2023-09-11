import { HttpClient } from '@angular/common/http';
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
    return this.httpClient.get<Book[]>(this.path + "Book/GetAllBooksWithImages", {withCredentials: true});
  }
  public getBookWithImage(bookId: number):Observable<any>{
    return this.httpClient.get<Book>(this.path + "Book/GetBookWithImage/"+bookId, {withCredentials: true});
  }
  public getBookStories(bookId: number): Observable<Story[]>{
    return this.httpClient.get<Story[]>
    (this.path + "Story/GetPlayableStoriesForBook/"+bookId, {withCredentials: true});
  }

}

