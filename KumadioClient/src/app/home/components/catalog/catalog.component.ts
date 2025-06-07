import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { BookCategory, ReturnBook } from 'src/app/entities';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';
import { SliderComponent } from '../slider/slider.component';
import { CommonModule } from '@angular/common';
import { BookCategoryPipe } from '../../pipes/category.pipe';
import { CatalogService } from '../../services/catalog.service';

@Component({
  selector: 'app-catalog',
  templateUrl: 'catalog.component.html',
  styleUrls: ['catalog.component.scss'],
  imports: [SliderComponent, CommonModule, BookCategoryPipe]
})
export class CatalogComponent implements OnInit {
  @ViewChild('slider', { static: true }) slider!: ElementRef;

  homePageCategories: number[] = [8, 6, 3];
  categories: { books: ReturnBook[], category: BookCategory }[] = [];

  ads = [
    { image: 'https://i.pinimg.com/736x/f2/ca/ae/f2caaeb3375ba55fb80ab633c1bb9785.jpg', alt: 'Banner 2' },
    { image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQXICWl6tskBwvFowpPtfNagBJCQSZVj4hDtA&s', alt: 'Banner 1' },
    { image: 'https://www.thebookwarehouse.com.au/wp-content/uploads/2021/09/image-30.png', alt: 'Banner 3' }
  ];

  activeAdIndex = 0;

  constructor(private catalogService: CatalogService, private spinnerService: LoadingSpinnerService) {}

  ngOnInit(): void {
    this.homePageCategories.forEach(category => {
      this.loadBooksFromCategory(category, 1, 15);
    });
  }

  loadBooksFromCategory(bookCategory: number, pageNumber: number, pageSize: number): void {
    this.spinnerService.setLoading(true);
    this.catalogService.getBooksFromCategory(bookCategory, pageNumber, pageSize).subscribe({
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
