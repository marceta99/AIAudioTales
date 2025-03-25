import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Category, ReturnBook, Toast, ToastIcon, ToastType } from 'src/app/entities';
import { Router } from '@angular/router';
import { catchError, debounceTime, filter, fromEvent, map, Observable, of, switchMap, tap } from 'rxjs';
import { SearchService } from '../../services/search.service';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';
import { ToastNotificationService } from 'src/app/common/services/toast-notification.service';
import { CatalogService } from '../../services/catalog.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-search',
  templateUrl: 'search.component.html',
  styleUrls: ['search.component.scss'],
  imports: [CommonModule]
})
export class SearchComponent implements OnInit, AfterViewInit {
  private activeCategory!: Category | undefined;
  private pageSize: number = 10;
  private pageNumber: number = 1;
  private noMoreBooks: boolean = false;
 
  public searchHistory: string[] = [];
  public searchedBooks: ReturnBook[] = [];
  public categories!: Category[];
  public activeSearchHistory: boolean = true;

  @ViewChild('mainSearchContainer') mainSearchContainer!: ElementRef;

  constructor(
    private searchService: SearchService,
    private catalogService: CatalogService,
    private spinnerService: LoadingSpinnerService,
    private toasterService: ToastNotificationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.getSearchHistory();
    this.getCategories();

    this.searchService.isSearchActive.next(true);

    this.searchService.searchedBooks$.subscribe((searchedBooks: ReturnBook[]) => {
      console.log("searachedbooks")
      this.activeSearchHistory = false;
      this.activeCategory = undefined;
      this.searchedBooks = searchedBooks
    })
  }

  ngAfterViewInit(): void {
    fromEvent(this.mainSearchContainer.nativeElement, 'scroll')
      .pipe(
        tap(() => console.log("Scroll event fired")),
        filter(() => !this.noMoreBooks),
        debounceTime(300),
        map(() => ({
          scrollTop: this.mainSearchContainer.nativeElement.scrollTop,
          scrollHeight: this.mainSearchContainer.nativeElement.scrollHeight,
          clientHeight: this.mainSearchContainer.nativeElement.clientHeight,
        })),
        tap(({ scrollTop, scrollHeight, clientHeight }) => {
          console.log(`scrollTop: ${scrollTop}, scrollHeight: ${scrollHeight}, clientHeight: ${clientHeight}`);
        }),
        filter(({ scrollTop, scrollHeight, clientHeight }) => scrollTop + clientHeight >= scrollHeight - 100),
        switchMap(() => {
          console.log("Fetching more books...");
          if (this.activeCategory) {
            return this.appendBooksFromCategory(this.activeCategory.id);
          } else {
            return this.appendSearchedBooks();
          }
        })
      )
      .subscribe();
  }

  private getSearchHistory(): void{
    this.searchService.getSearchHistory().subscribe({
      next: (searchHistory : string[] ) => {
        this.searchHistory = searchHistory;
      },
      error: error => {
        console.error('There was an error!', error);
      }
    })
  }

  private appendSearchedBooks(): Observable<ReturnBook[]> {
    return this.searchService.searchBooks(this.searchService.searchTerm, this.pageNumber++, this.pageSize).pipe(
      tap((books: ReturnBook[]) => {
        console.log("Fetched searched books:", books);
        if (books.length < this.pageSize) {
          this.noMoreBooks = true;
        }
        // Append new books to the existing array
        this.searchedBooks = [...this.searchedBooks, ...books];
        // Update the BehaviorSubject
        this.searchService.searchedBooks.next(this.searchedBooks);
      }),
      catchError(error => {
        console.error('There was an error!', error);
        // Handle error appropriately
        return of([]); // Return an empty array to keep the Observable chain alive
      })
    );
  }
  
  private getCategories(): void {
    this.catalogService.getAllCategories().subscribe((categories: Category[]) => {
      this.categories = categories;
    })
  }

  private appendBooksFromCategory(category: number): Observable<ReturnBook[]> {
    return this.catalogService.getBooksFromCategory(category, this.pageNumber++, this.pageSize).pipe(
      tap((books: ReturnBook[]) => {
        console.log("Fetched category books:", books);
        if (books.length < this.pageSize) {
          this.noMoreBooks = true;
        }
        // Append new books to the existing array
        this.searchedBooks = [...this.searchedBooks, ...books];
        // Update the BehaviorSubject
        this.searchService.searchedBooks.next(this.searchedBooks);
      }),
      catchError(error => {
        console.error('There was an error!', error);
        // Handle error appropriately
        const toast: Toast = {
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error,
        };
        this.spinnerService.setLoading(false);
        this.toasterService.show(toast);
        return of([]); // Return an empty array to keep the Observable chain alive
      })
    );
  }

  public getBooksFromCategory(category: number): void {
    this.pageNumber = 1; // Reset page number
    this.noMoreBooks = false; // Reset the flag
    this.activeCategory = this.categories.find(cat => cat.id === category);
    this.spinnerService.setLoading(true);
    this.catalogService.getBooksFromCategory(category, this.pageNumber++, this.pageSize)
      .subscribe({
        next: (books: ReturnBook[]) => {
          this.searchedBooks = books;
          this.searchService.searchedBooks.next(books);
          this.spinnerService.setLoading(false);
        },
        error: error => {
          console.error('There was an error!', error);
          const toast: Toast = {
            text: "We're sorry! An error occurred. Please try again later.",
            toastIcon: ToastIcon.Error,
            toastType: ToastType.Error
          };
          this.spinnerService.setLoading(false);
          this.toasterService.show(toast);
        }
      });
  }
  
  public selectBook(book: ReturnBook): void {
    this.searchService.saveSearchTerm(book.title);
    this.searchService.isSearchActive.next(false);
    this.router.navigate(['/home/books',book.id])
  }

  public onHistoryItemClick(searchTerm: string): void {
    // 3) This sets the side nav’s input to this value
    this.searchService.searchTerm = searchTerm;
    // If you also want to instantly trigger the search, it’ll happen
    // because Sidenav is listening to `searchControl.valueChanges`.
  }
}
