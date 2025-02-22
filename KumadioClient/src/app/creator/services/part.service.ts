import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Book, CreateBook, PartTree, ReturnPart } from "src/app/entities";

@Injectable()
export class PartService {
    /*
    public getBookTree(bookId: number): Observable<PartTree> {
    return this.httpClient.get<PartTree>(this.path+`Books/GetBookTree/${bookId}`);
    }

    public getPart(partId: number): Observable<ReturnPart> {
    return this.httpClient.get<ReturnPart>(this.path+`Books/GetPart/${partId}`);
    }

    public getCreatorBooks(): Observable<Book[]> {
    return this.httpClient.get<Book[]>(this.path+"Books/GetCreatorBooks");
    }

      public createBook(newBook: CreateBook): Observable<number>{
        return this.httpClient.post<number>(this.path + "Books/AddBook", newBook);
      }
    
      public addRootPart(formData: FormData): Observable<ReturnPart>{
        return this.httpClient.post<ReturnPart>(this.path + "Books/AddRootPart", formData, { withCredentials: true});
      }
    
      public addBookPart(formData: FormData): Observable<ReturnPart>{
        return this.httpClient.post<ReturnPart>(this.path + "Books/AddBookPart", formData, {withCredentials: true});
      }*/
}