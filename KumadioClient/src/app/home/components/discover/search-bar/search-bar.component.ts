import { ChangeDetectorRef, Component, OnInit } from '@angular/core';

import { Observable, debounceTime, distinctUntilChanged, filter, switchMap, tap } from 'rxjs';
import { Book, ReturnBook, SearchedBooks } from 'src/app/entities';
import { FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-search-bar',
  templateUrl: './search-bar.component.html',
  styleUrls: ['./search-bar.component.scss'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class SearchBarComponent {
  searchControl = new FormControl();
  filteredBooks!: ReturnBook[]; 
  isInputActive: boolean = false;
  searchHistory: string[] = [];
  searchIsNotEmptyString: boolean = false;

  constructor(
    private spinnerService: LoadingSpinnerService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
    this.cdr.detectChanges();

    this.fetchSearchHistory();

    /*this.searchControl.valueChanges.pipe(
      debounceTime(300),                                                                            // Wait for 300ms pause in events
      distinctUntilChanged(),                                                                       // Only emit if value is different from previous value
      filter((searchTerm: string) => {
        this.searchIsNotEmptyString = searchTerm.trim().length > 0;
        this.searchIsNotEmptyString ? this.searchHistory = [] : this.fetchSearchHistory();          //if search input is empty then show search history
        return this.searchIsNotEmptyString;                                                         // if search term is empty string or just empty white space return false so that http request to the backend is not send
      }),
      switchMap((searchTerm : string) => {                                                          // we use switch map because If a new term is entered while a previous search request is still in progress, that HTTP request is cancelled, and only the latest search is processed.
        return this.bookService.searchBooks(searchTerm,1,10)
      })
    ).subscribe({
      next: (books : ReturnBook[]) => {
        this.filteredBooks = books
      },
      error: error => {
        console.error('There was an error!', error);
      }
    })*/
  }

  private fetchSearchHistory(): void{
   /* this.bookService.getSearchHistory().subscribe({
      next: (searchHistory : string[] ) => {
        console.log("searchHistory",searchHistory);
        this.searchHistory = searchHistory;
        this.filteredBooks = [];
      },
      error: error => {
        console.error('There was an error!', error);
      }
    })*/
  }

  public searchSubmit(): void{
    /*if(this.searchIsNotEmptyString){
      const searchTerm = this.searchControl.value;
      this.bookService.saveSearchTerm(searchTerm);
      this.fetchBooks(searchTerm);
    }*/
  }

  public searchFromInputList(searchTerm: string): void{
    /*console.log("search happend");
    this.bookService.saveSearchTerm(searchTerm);
    this.fetchBooks(searchTerm);*/
  }

  public searchFromSearchHistory(searchTerm: string): void{
     console.log("search happend");
     this.fetchBooks(searchTerm);
  }

  private fetchBooks(searchTerm: string): void{
    /*this.isInputActive = false;
    this.bookService.searchBooks(searchTerm,1,10).subscribe({
      next: (books : ReturnBook[] ) => {
        console.log(books)
        const searchBooks : SearchedBooks = {
          searchTerm: searchTerm,
          books: books
        }
        this.bookService.libraryBooks.next(searchBooks);
      },
      error: error => {
        console.error('There was an error!', error);
      }
    })*/
  }

  focus(){
    this.isInputActive = true;
  }
  blur(){
    setTimeout(() => {
      this.isInputActive = false;
    }, 100);
  }
}
