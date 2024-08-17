import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, finalize } from 'rxjs';
import { Basket, Book, ReturnPart, Category, CreateBook, PartTree, Purchase, PurchasedBook, SearchedBooks } from 'src/app/entities';
import { environment } from 'src/environment/environment';
import { LoadingSpinnerService } from './loading-spinner.service';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private path = environment.apiUrl;
  libraryBooks = new Subject<SearchedBooks>();
  basket = new BehaviorSubject<Basket>({
    basketItems : [],
    totalPrice : 0
  });
  purchasedBooks: BehaviorSubject<PurchasedBook[]> = new BehaviorSubject<PurchasedBook[]>([]);
  currentBookIndex: Subject<number> = new Subject<number>();

  constructor(private httpClient: HttpClient, private spinnerService: LoadingSpinnerService) { }

  //#region GET

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
    (this.path + "Books/GetBook/"+bookId).pipe(
      finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }

  public userHasBook(bookId: number):Observable<boolean>{
    return this.httpClient.get<boolean>(this.path + "Books/UserHasBook/"+bookId);
  }

  public isBasketItem(bookId: number):Observable<boolean>{
    return this.httpClient.get<boolean>(this.path + "Books/IsBasketItem/"+bookId);
  }

  public getPurchasedBooks():Observable<PurchasedBook[]>{
    this.spinnerService.setLoading(true);

    return this.httpClient.get<PurchasedBook[]>(this.path + "Books/GetPurchasedBooks").pipe(
      finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }

  public getPurchasedBook(bookId: number):Observable<PurchasedBook>{
    this.spinnerService.setLoading(true);

    return this.httpClient.get<PurchasedBook>(this.path + "Books/GetPurchasedBook/"+ bookId).pipe(
      finalize(() => this.spinnerService.setLoading(false)) //finalize operator has guaranteed execution, so it is called regardless where it is error or successfull response
    );
  }

  public searchBooks(searchTerm: string, pageNumber: number, pageSize: number): Observable<Book[]> {
    const params = new HttpParams()
            .set('searchTerm', searchTerm)
            .set('pageNumber', pageNumber.toString())
            .set('pageSize', pageSize.toString());

    return this.httpClient.get<Book[]>(this.path+"Books/Search", { params});
  }

  public getSearchHistory(): Observable<string[]> {
    return this.httpClient.get<string[]>(this.path+"Books/GetSearchHistory");
  }

  public getAllCategories(): Observable<Category[]> {
    return this.httpClient.get<Category[]>(this.path+"Books/GetAllCategories");
  }

  public getBasket(): void{
    this.httpClient
               .get<Basket>(this.path+"Books/GetBasket")
               .subscribe({
                next: (basket: Basket) => {
                  this.basket.next(basket)
              },
                error: error => {
                  console.error('There was an error!', error);
              }})  
  }

  public getBookTree(bookId: number): Observable<PartTree> {
    return this.httpClient.get<PartTree>(this.path+`Books/GetBookTree/${bookId}`);
  }

  public getPart(partId: number): Observable<ReturnPart> {
    return this.httpClient.get<ReturnPart>(this.path+`Books/GetPart/${partId}`);
  }

  public getCreatorBooks(): Observable<Book[]> {
    return this.httpClient.get<Book[]>(this.path+"Books/GetCreatorBooks");
  }

  //#endregion

  //#region  POST
  
  public purchaseBook(purchase: Purchase){
    return this.httpClient.post(this.path + "Books/PurchaseBook", purchase, {withCredentials: true, responseType: 'text'});
  }
  
  public saveSearchTerm(searchTerm: string){
    const params = new HttpParams().set('searchTerm', searchTerm);

    this.httpClient.post(this.path+"Books/SaveSearchTerm", searchTerm, {params}).subscribe((res:any)=>console.log(res));
  }

  public addBasketItem(bookId: number): Observable<Basket>{
    const params = new HttpParams().set('bookId', bookId);
    return this.httpClient.post<Basket>(this.path+"Books/AddBasketItem", {},{ params });
  }

  public createBook(newBook: CreateBook): Observable<number>{
    return this.httpClient.post<number>(this.path + "Books/AddBook", newBook);
  }

  public addRootPart(formData: FormData): Observable<ReturnPart>{
    return this.httpClient.post<ReturnPart>(this.path + "Books/AddRootPart", formData, { withCredentials: true});
  }

  public addBookPart(formData: FormData): Observable<ReturnPart>{
    return this.httpClient.post<ReturnPart>(this.path + "Books/AddBookPart", formData, {withCredentials: true});
  }

  //#endregion

  //#region PATCH

  public nextPart(bookId: number, nextPartId: number): Observable<PurchasedBook>{
    const nextPart = {
      "bookId": bookId,
      "nextPartId": nextPartId
    }
    return this.httpClient.patch<PurchasedBook>(this.path + "Books/NextPart", nextPart);
  }

  public activateQuestions(bookId: number): Observable<void>{
    return this.httpClient.patch<void>(this.path + "Books/ActivateQuestions/"+bookId, {});
  }

  public updateProgress(bookId: number, playingPosition?: number, nextBookId?: number): Observable<void> {
    const progress: any = { bookId };

    // Conditionally add optional properties if they have values
    if (playingPosition !== undefined) {
      progress.playingPosition = playingPosition;
    }
    
    if (nextBookId !== undefined) {
      progress.nextBookId = nextBookId;
    }

    return this.httpClient.patch<void>(this.path + "Books/UpdateProgress", progress);
  }

  public startBookAgain(bookId: number): Observable<PurchasedBook> {
    return this.httpClient.patch<PurchasedBook>(this.path + "Books/StartBookAgain/"+bookId, {});
  }
  

  //#endregion

  //#region DELETE

  public removeBasketItem(itemId: number): Observable<Basket>{
    const params = new HttpParams().set('itemId', itemId);
    return this.httpClient.delete<Basket>(this.path+"Books/RemoveBasketItem",{ params, withCredentials : true});
  }

  //#endregion 
}

