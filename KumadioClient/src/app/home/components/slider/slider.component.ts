import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ReturnBook } from 'src/app/entities';

@Component({
  selector: 'app-slider',
  templateUrl: './slider.component.html',
  styleUrls: ['./slider.component.scss'],
  imports: [CommonModule]
})
export class SliderComponent implements OnInit, AfterViewInit{
  private wrapper!: any;
  private carousel!: any;
  private firstCardWidth!: any;
  private arrowBtns!: any;
  private carouselChildrens : any[] = [];
  private isDragging: boolean = false;
  private isAutoPlay: boolean = true;
  private startX! : any;
  private startScrollLeft: any;
  private timeoutId: any;
  
  @Input() books!: ReturnBook[];
  @Input() size: 'small' | 'big' = 'small'; 
  @Input() sliderId!: string; // Unique identifier for each slider instance

  constructor(private elRef: ElementRef, private router: Router) {}

  ngAfterViewInit(): void {
    this.initializeElements();
  }

  ngOnInit(): void {}

  navigateToBook(bookId: number){
    this.router.navigate(['/home/books',bookId])
  }

  initializeElements(){
    this.wrapper = this.elRef.nativeElement.querySelector(".wrapper" + this.sliderId);
    this.carousel = this.elRef.nativeElement.querySelector(".carousel" + this.sliderId);
    this.firstCardWidth = this.elRef.nativeElement.querySelector(".card" + this.sliderId)?.offsetWidth;
    this.arrowBtns = this.elRef.nativeElement.querySelectorAll("#i" + this.sliderId);
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
