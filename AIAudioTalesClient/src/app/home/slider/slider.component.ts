import { AfterViewInit, Component, ElementRef, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BooksPaginated } from 'src/app/entities';

@Component({
  selector: 'app-slider',
  templateUrl: './slider.component.html',
  styleUrls: ['./slider.component.scss']
})
export class SliderComponent implements OnInit, AfterViewInit{
  wrapper!: any;
  carousel!: any;
  firstCardWidth!: any;
  arrowBtns!: any;
  carouselChildrens : any[] = [];
  isDragging: boolean = false;
  isAutoPlay: boolean = true;
  startX! : any;
  startScrollLeft: any;
  timeoutId: any;
  
  @Input() bookPaginated!: BooksPaginated;
  @Input() size: 'small' | 'big' = 'small'; 

  constructor(private elRef: ElementRef, private router: Router) {}

  ngAfterViewInit(): void {
    this.helperFunction(this.bookPaginated.booksCategory);
  }

  ngOnInit(): void {}

  navigateToBook(bookId: number){
    this.router.navigate(['/home/books',bookId])
  }

  helperFunction(booksCategory: number){
    this.wrapper = this.elRef.nativeElement.querySelector(".wrapper"+booksCategory);
    this.carousel = this.elRef.nativeElement.querySelector(".carousel"+booksCategory);
    this.firstCardWidth = this.elRef.nativeElement.querySelector(".card"+booksCategory)?.offsetWidth;
    this.arrowBtns = this.elRef.nativeElement.querySelectorAll("#i"+booksCategory);

  }
  // Add event listeners for the arrow buttons to scroll the this.carousel left and right
  arrowButtonsClick(button :HTMLElement){
    const id = button.id;
    console.log(id)
    this.carousel.scrollLeft += id.includes("left") ? -this.firstCardWidth : this.firstCardWidth;
  }

  dragStart(e : any){
      this.isDragging = true;
      this.carousel.classList.add("dragging");
      // Records the initial cursor and scroll position of the this.carousel
      this.startX = e.pageX;
      this.startScrollLeft = this.carousel.scrollLeft;
  }

  dragging(e: any){
      if(!this.isDragging) return; // if isDragging is false return from here
      // Updates the scroll position of the this.carousel based on the cursor movement
      this.carousel.scrollLeft = this.startScrollLeft - (e.pageX - this.startX);
  }

  dragStop() {
      this.isDragging = false;
      this.carousel.classList.remove("dragging");
  }

}
