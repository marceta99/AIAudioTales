import { Component, OnInit } from '@angular/core';
import { Book } from 'src/app/entities';
import { Router } from '@angular/router';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';
import { CreatorService } from '../../services/creator.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-my-books',
  templateUrl: 'my-books.component.html',
  styleUrls: ['my-books.component.scss'],
  imports: [CommonModule]
})
export class MyBooksComponent implements OnInit {
  public books: Book[] = [];

  constructor(private spinnerService: LoadingSpinnerService,
              private creatorService: CreatorService,
              private router: Router) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
  
    this.creatorService.getCreatorBooks().subscribe((books: Book[])=> this.books = books);
  }
  navigateToTree(bookId: number){
    this.router.navigate(['/home/book-tree',bookId])
  }
}
