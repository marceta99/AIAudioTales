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

  isDragging = false;
  isAutoPlay = true;
  startX! : any;
  startScrollLeft: any;
  timeoutId: any;
  @Input() bookPaginated!: BooksPaginated;

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

    //this.carouselChildrens = [...this.carousel?.children] as any;
    // Get the number of cards that can fit in the this.carousel at once
    //let cardPerView = Math.round(this.carousel.offsetWidth / this.firstCardWidth);

    // Insert copies of the last few cards to beginning of this.carousel for infinite scrolling
    /*this.carouselChildrens.slice(-cardPerView).reverse().forEach(card => {
      this.carousel.insertAdjacentHTML("afterbegin", card.outerHTML);
    });

    // Insert copies of the first few cards to end of this.carousel for infinite scrolling
    this.carouselChildrens.slice(0, cardPerView).forEach(card => {
      this.carousel.insertAdjacentHTML("beforeend", card.outerHTML);
    });

    // Scroll the this.carousel at appropriate postition to hide first few duplicate cards on Firefox
    this.carousel.classList.add("no-transition");
    this.carousel.scrollLeft = this.carousel.offsetWidth;
    this.carousel.classList.remove("no-transition");*/

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

  infiniteScroll(){
      // If the this.carousel is at the beginning, scroll to the end
      if(this.carousel.scrollLeft === 0) {
          console.log("levo")
          this.carousel.classList.add("no-transition");
          this.carousel.scrollLeft = this.carousel.scrollWidth - (2 * this.carousel.offsetWidth);
          this.carousel.classList.remove("no-transition");
      }
      // If the this.carousel is at the end, scroll to the beginning
      else if(Math.ceil(this.carousel.scrollLeft) === this.carousel.scrollWidth - this.carousel.offsetWidth) {
          console.log("desno")
          this.carousel.classList.add("no-transition");
          this.carousel.scrollLeft = this.carousel.offsetWidth;
          this.carousel.classList.remove("no-transition");
      }

      // Clear existing timeout & start autoplay if mouse is not hovering over this.carousel
      //clearTimeout(this.timeoutId);
      //if(!this.wrapper.matches(":hover")) this.autoPlay();
  }

  autoPlay(){
      if(window.innerWidth < 800 || !this.isAutoPlay) return; // Return if window is smaller than 800 or isAutoPlay is false
      // Autoplay the this.carousel after every 2500 ms
      this.timeoutId = setTimeout(() => this.carousel.scrollLeft += this.firstCardWidth, 2500);
  }
  clearTimeout(){
    clearTimeout(this.timeoutId);
  }
}
