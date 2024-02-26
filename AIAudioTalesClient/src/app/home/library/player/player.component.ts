import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PurchasedBook } from 'src/app/entities';
import { BookService } from '../../services/book.service';

@Component({
  selector: 'app-player',
  templateUrl: './player.component.html',
  styleUrls: ['./player.component.scss']
})
export class PlayerComponent implements OnInit{
  book!: PurchasedBook;

  constructor(private route : ActivatedRoute, private bookService : BookService) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      const id = +params['bookId'];

      this.bookService.getPurchasedBook(id).subscribe({
        next: (book : PurchasedBook ) => {
          this.book = book;
      },
        error: error => {
          console.error('There was an error!', error);
      }
      })
    });


  }

}
