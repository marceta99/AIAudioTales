import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { finalize, Observable } from "rxjs";
import { LoadingSpinnerService } from "src/app/common/services/loading-spinner.service";
import { Category, DtoBookPreview, ReturnBook } from "src/app/entities";
import { environment } from "src/environments/environment";

@Injectable()
export class CatalogService {
  private baseUrl = environment.apiUrl + '/catalog';

  constructor(private http: HttpClient, private spinnerService: LoadingSpinnerService) {}

  public getBooksFromCategory(categoryId: number, page: number, pageSize: number): Observable<ReturnBook[]>{
    const params = new HttpParams()
      .set('categoryId', categoryId)
      .set('page', page)
      .set('pageSize', pageSize);

    return this.http.get<ReturnBook[]>(`${this.baseUrl}/books`, {params, withCredentials : true});
  }

  public getBookWithPreivew(bookId: number): Observable<DtoBookPreview>{
    this.spinnerService.setLoading(true);

    return this.http.get<DtoBookPreview>(`${this.baseUrl}/books/${bookId}`, {withCredentials : true})
    .pipe(
      finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }

  public getAllCategories(): Observable<Category[]> {
      return this.http.get<Category[]>(`${this.baseUrl}/categories`, {withCredentials : true});
  }
}