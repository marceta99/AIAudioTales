import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { BookService } from '../services/book.service';
import { BookCategory, ReturnBook } from 'src/app/entities';
import { LoadingSpinnerService } from '../services/loading-spinner.service';

@Component({
  selector: 'app-books',
  templateUrl: './books.component.html',
  styleUrls: ['./books.component.scss']
})
export class BooksComponent implements OnInit {
  @ViewChild('slider', { static: true }) slider!: ElementRef;

  homePageCategories: number[] = [5, 6, 2];
  categories: { books: ReturnBook[], category: BookCategory }[] = [];

  ads = [
    { image: 'https://www.catholicmom.com/hubfs/20240104%20CRangel%201.png', alt: 'Banner 2' },
    { image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQXICWl6tskBwvFowpPtfNagBJCQSZVj4hDtA&s', alt: 'Banner 1' },
    { image: 'https://www.thebookwarehouse.com.au/wp-content/uploads/2021/09/image-30.png', alt: 'Banner 3' }
  ];

  activeAdIndex = 0;

  constructor(private bookService: BookService, private spinnerService: LoadingSpinnerService) {}

  ngOnInit(): void {
    this.homePageCategories.forEach(category => {
      this.loadBooksFromCategory(category, 1, 15);
    });
  }

  loadBooksFromCategory(bookCategory: number, pageNumber: number, pageSize: number): void {
    this.spinnerService.setLoading(true);
    this.bookService.getBooksFromCategory(bookCategory, pageNumber, pageSize).subscribe({
      next: (books: ReturnBook[]) => {
        const category = {
          books: books,
          category: bookCategory as BookCategory
        };
        this.categories.push(category);
      },
      complete: () => this.spinnerService.setLoading(false),
      error: error => {
        console.error('There was an error!', error);
        this.spinnerService.setLoading(false);
      }
    });
  }

  onScroll(): void {
    const slider = this.slider.nativeElement;
    const scrollLeft = slider.scrollLeft;
    const width = slider.offsetWidth;

    // Calculate the nearest index
    this.activeAdIndex = Math.round(scrollLeft / width);
  }

  onScrollEnd(): void {
    const slider = this.slider.nativeElement;
    const width = slider.offsetWidth;

    // Snap to the nearest ad
    slider.scrollTo({ left: this.activeAdIndex * width, behavior: 'smooth' });
  }

  scrollToAd(index: number): void {
    const slider = this.slider.nativeElement;
    const width = slider.offsetWidth;

    slider.scrollTo({ left: index * width, behavior: 'smooth' });
    this.activeAdIndex = index;
  }
}
