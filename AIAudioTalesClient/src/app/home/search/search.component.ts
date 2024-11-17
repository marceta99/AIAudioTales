import { Component, OnInit } from '@angular/core';
import { BookService } from '../services/book.service';
import { SearchService } from '../services/search.service';
import { ReturnBook } from 'src/app/entities';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent implements OnInit {
  searchHistory: string[] = [];
  searchedBooks: ReturnBook[] = [];
  categories: string[] = ['Artists', 'Songs', 'Playlists', 'Albums', 'Podcasts', 'Shows', 'Representers'];

  constructor(private searchService: SearchService) { }

  ngOnInit(): void {
    this.getSearchHistory();

    this.searchService.searchedBooks$.subscribe((searchedBooks: ReturnBook[]) => {
      console.log("subject event ", searchedBooks)
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
}
