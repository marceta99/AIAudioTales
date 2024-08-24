import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { Book, Category, ReturnBook, SearchedBooks, Toast, ToastIcon, ToastType } from 'src/app/entities';
import { BookService } from '../services/book.service';
import { ToastNotificationService } from '../services/toast-notification.service';
import { Router } from '@angular/router';
import { debounceTime, filter, fromEvent, map, switchMap, tap } from 'rxjs';

@Component({
  selector: 'app-discover',
  templateUrl: './discover.component.html',
  styleUrls: ['./discover.component.scss']
})
export class DiscoverComponent implements OnInit, AfterViewInit{
  books! : ReturnBook[];
  bookCategories!: Category[];
  selectedCategory!: Category;
  pageSize: number = 10;
  pageNumber: number = 1;
  currentCategory: number = 1;
  searchTerm: string = "";
  isSearchFromTerm: boolean = false;
  noMoreBooks: boolean = false; //there are no more books to load for that category from the server

  @ViewChild('booksContainer') booksContainer!: ElementRef;

  constructor(
    private bookService : BookService,
    private spinnerService: LoadingSpinnerService,
    private notificationService: ToastNotificationService,
    private router: Router) {}

  ngAfterViewInit(): void{
    //don't send GET request if already all books are loaded
    fromEvent(this.booksContainer.nativeElement, 'scroll')
    .pipe(
      filter(()=> !this.noMoreBooks),
      debounceTime(300), // Wait for 200 ms of inactivity
      map((event: any) => ({
        scrollTop: event.target.scrollTop,
        scrollHeight: event.target.scrollHeight,
        clientHeight: event.target.clientHeight,
      })),
      /*event.target.scrollTop gives the number of pixels that the content of .books is scrolled vertically.
        event.target.clientHight viewable height of an element in pixels, including vertical padding, but not including borders, margins, or horizontal scrollbars (if present).
        event.target.scrollHeight is the total scrollable height of .books, including content not visible on the screen.
        When scrollTop + offsetHeight is greater than or equal to scrollHeight, it means the user has scrolled to the bottom.*/
      filter(({ scrollTop, scrollHeight, clientHeight }) => scrollTop + clientHeight >= scrollHeight - 50), // Near bottom, that is why is -50
      switchMap(() => {
        if(this.isSearchFromTerm) {
          return this.bookService.searchBooks(this.searchTerm, this.pageNumber++, this.pageSize).pipe(tap(this.fetchingOperatorHelper()))
        } else {
          return this.bookService.getBooksFromCategory(this.currentCategory, this.pageNumber++, this.pageSize).pipe(tap(this.fetchingOperatorHelper()))
        }
      }
      )
    )
    .subscribe();

  }

  ngOnInit(): void {
    this.spinnerService.setLoading(true);

    this.getBooksFromCategory(1);

    this.bookService.libraryBooks.subscribe((searchedBooks: SearchedBooks) => {
      console.log("from search bar ")
      this.books = searchedBooks.books;
      this.searchTerm = searchedBooks.searchTerm;
      this.isSearchFromTerm = true;
      this.pageNumber=1;
      this.noMoreBooks = false;
    })

    this.bookService.getAllCategories().subscribe((categories: Category[]) => {
      console.log(categories);
      this.bookCategories = categories;
    })
  }

  getBooksFromCategory(category: number){
    this.isSearchFromTerm = false;
    this.currentCategory = category;
    this.pageNumber = 1;
    this.noMoreBooks= false;
    this.bookService.getBooksFromCategory(category, this.pageNumber, this.pageSize).subscribe({
      next: (books : ReturnBook[] ) => {
        console.log("books from category",books)
        this.books = books;
        this.spinnerService.setLoading(false);
    },
      error: error => {
        console.error('There was an error!', error);
        const toast: Toast = {
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error
        }
        this.spinnerService.setLoading(false);
        this.notificationService.show(toast);
      }
    })
  }

  navigateToBook(bookId: number){
    this.router.navigate(['/home/books',bookId])
  }

  private fetchingOperatorHelper(){
      return {
        next: (books : ReturnBook[]) => {
          console.log(books)
          if (books.length < this.pageSize) {
            this.noMoreBooks = true;
          }
          // Append books to your books array here
          this.books = [...this.books, ...books];
        },
        error: (error: any) => {
          console.error('There was an error!', error);
          const toast: Toast = {
            text: "We're sorry! An error occurred. Please try again later.",
            toastIcon: ToastIcon.Error,
            toastType: ToastType.Error
          }
          this.notificationService.show(toast);
        }
      }

  }

}
