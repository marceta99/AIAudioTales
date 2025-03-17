import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Book, Category, CreateBook, PartTree, ReturnPart } from "src/app/entities";
import { environment } from "src/environments/environment";

@Injectable()
export class CreatorService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  public getBookTree(bookId: number): Observable<PartTree> {
  return this.http.get<PartTree>(`${this.baseUrl}/catalog/part-tree/${bookId}`);
  }

  public getPart(partId: number): Observable<ReturnPart> {
  return this.http.get<ReturnPart>(`${this.baseUrl}/catalog/parts/${partId}`);
  }

  public getCreatorBooks(): Observable<Book[]> {
  return this.http.get<Book[]>(`${this.baseUrl}/library/creator/books`);
  }

  public createBook(newBook: CreateBook): Observable<number>{
    return this.http.post<number>(`${this.baseUrl}/editor/book`, newBook);
  }

  public addRootPart(formData: FormData): Observable<ReturnPart>{
    return this.http.post<ReturnPart>(`${this.baseUrl}/editor/root-part`, formData, { withCredentials: true});
  }

  public addBookPart(formData: FormData): Observable<ReturnPart>{
    return this.http.post<ReturnPart>(`${this.baseUrl}/editor/part`, formData, {withCredentials: true});
  }

  public getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/catalog/categories`);
  }
}