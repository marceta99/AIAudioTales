import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent implements OnInit {
  searchHistory: string[] = [];

  constructor(private bookService: BookService) { }

  ngOnInit(): void {
    this.fetchSearchHistory();
  }

  private fetchSearchHistory(): void{
    this.bookService.getSearchHistory().subscribe({
      next: (searchHistory : string[] ) => {
        console.log("searchHistory",searchHistory);
        this.searchHistory = searchHistory;
      },
      error: error => {
        console.error('There was an error!', error);
      }
    })
  }
}
