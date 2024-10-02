import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { ReturnBook } from 'src/app/entities';

@Component({
  selector: 'app-book-list',
  templateUrl: './book-list.component.html',
  styleUrls: ['./book-list.component.scss']
})
export class BookListComponent {
  @Input() noMoreBooks: boolean = false; 
  @Input() public books!: ReturnBook[];
  
  constructor(private router: Router) { }

  navigateToBook(bookId: number){
    this.router.navigate(['/home/books',bookId])
  }

}
