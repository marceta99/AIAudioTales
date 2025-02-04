import { Component, OnInit } from '@angular/core';
import { LoadingSpinnerService } from './services/loading-spinner.service';
import { Observable } from 'rxjs';
import { BookService } from './services/book.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit{
  isLoading!: Observable<boolean>;
  playerActive$!: Observable<boolean>;

  constructor(private spinnerService: LoadingSpinnerService, private bookService: BookService) { }

  ngOnInit():void {
    this.playerActive$ = this.bookService.playerActive$;
    
    this.isLoading = this.spinnerService.loading$;
    this.bookService.getBasket();
  }

}
