import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { Observable, debounceTime, distinctUntilChanged, filter, switchMap, tap } from 'rxjs';
import { Book } from 'src/app/entities';
import { BookService } from '../services/book.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-discover',
  templateUrl: './discover.component.html',
  styleUrls: ['./discover.component.scss']
})
export class DiscoverComponent implements OnInit{
  books! : Book[];


constructor(private bookService : BookService) {

}

  ngOnInit(): void {
    this.bookService.libraryBooks.subscribe((books: Book[])=>{
      this.books= books;
    })
  }

}