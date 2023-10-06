import { ChangeDetectorRef, Component } from '@angular/core';
import { BookService } from './services/book.service';
import { Book } from '../entities';
import { DomSanitizer } from '@angular/platform-browser';
import { SideNavToggle } from './sidenav/sidenav.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {

  booksWithImages : any;
  constructor(private bookService: BookService, private _sanitizer: DomSanitizer) { }

  isSideNavCollapsed = false;
  screenWidth = 0;

  ngOnInit():void{
  this.bookService.getAllBooksWithImages().subscribe({
    next :(booksWithImages:any) => {
      console.log(booksWithImages);


      booksWithImages.forEach((book: any) => {
        var img = this._sanitizer.bypassSecurityTrustResourceUrl('data:image/jpg;base64,'
        + book.imageData);
        book.photo = img;
      });
      this.booksWithImages=booksWithImages;
    },
    error :(error:any) => {
        console.log(error);
      }
});}

  onToggleSideNav(data: SideNavToggle): void{
    this.screenWidth = data.screenWidth;
    this.isSideNavCollapsed = data.collapsed;
  }

}
