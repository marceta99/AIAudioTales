import { Component } from '@angular/core';
import { BookService } from '../services/book.service';
import { Story } from 'src/app/entities';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.css']
})
export class BookComponent {
  stories! : Story[];

  constructor(private bookService: BookService) {}

  ngOnInit():void{

    this.bookService.getBookStories(1).subscribe({
      next :(bookStories:Story[]) => {
          console.log(bookStories);
          this.stories= bookStories;
      },
      error :(error:any) => {
          console.log(error);
        }
  });
 }

}
