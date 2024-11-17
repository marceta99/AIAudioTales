import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { SearchService } from '../services/search.service';
import { Category, ReturnBook, Toast, ToastIcon, ToastType } from 'src/app/entities';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { ToastNotificationService } from '../services/toast-notification.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent implements OnInit {
  searchHistory: string[] = [];
  searchedBooks: ReturnBook[] = [];
  categories!: Category[];
  pageSize: number = 10;
  pageNumber: number = 1;

  constructor(
    private searchService: SearchService,
    private bookService: BookService,
    private spinnerService: LoadingSpinnerService,
    private toasterService: ToastNotificationService
  ) { }

  ngOnInit(): void {
    this.getSearchHistory();
    this.getCategories();

    this.searchService.searchedBooks$.subscribe((searchedBooks: ReturnBook[]) => {
      this.searchedBooks = searchedBooks
    })
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

  private getCategories(): void {
    this.bookService.getAllCategories().subscribe((categories: Category[]) => {
      this.categories = categories;
    })
  }

  getBooksFromCategory(category: number){
    this.spinnerService.setLoading(false);
    this.bookService.getBooksFromCategory(category, this.pageNumber, this.pageSize).subscribe({
      next: (books : ReturnBook[] ) => {
        this.searchedBooks = books;
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
        this.toasterService.show(toast);
      }
    })
  }
}
