import { CommonModule } from '@angular/common';
import {
    Component,
    OnInit,
    ElementRef,
    ViewChild
  } from '@angular/core';
  import {
    FormArray,
    FormControl,
    FormGroup,
    ReactiveFormsModule,
    Validators,
  } from '@angular/forms';
  import { Router } from '@angular/router';
  
  /** 
   * We define each form control as a typed property.
   * FormControl<T> ensures that control’s value is T.
   */
  interface OnboardingFormControls {
    childAge: FormControl<number | null>;
    childGender: FormControl<string>;
    childInterests: FormArray<FormControl<boolean>>;
    preferredDuration: FormControl<string>;
  }
  
  @Component({
    selector: 'app-onboarding',
    templateUrl: 'onboarding.component.html',
    styleUrls: ['onboarding.component.scss'],
    imports: [CommonModule, ReactiveFormsModule]
  })
  export class OnboardingComponent implements OnInit {
    @ViewChild('slidesWrapper') slidesWrapper!: ElementRef<HTMLDivElement>;
  
    /** A typed FormGroup, referencing our OnboardingFormControls interface. */
    onboardingForm!: FormGroup<OnboardingFormControls>;
  
    /** 
     * For the multi-checkbox question:
     * We'll store possible interests in an array of strings.
     * The form array will have a boolean control for each string.
     */
    interests = [
      'Avantura',
      'Životinje',
      'Nauka i priroda',
      'Fantazija',
      'Sport',
      'Istorija',
      'Geografija',
      'Matematika i logičke igre',
      'Priče sa moralnim poukama',
      'Humor i zabava'
    ];
  
    // We can store slides indexes for a “horizontal slider” approach
    slides = [0, 1, 2, 3]; // 4 slides total
    currentIndex = 0;
    animating = false;
  
    // pointer event tracking
    private startX = 0;
    private currentX = 0;
    private isDragging = false;
    private threshold = 50; // px threshold for swipe
  
    constructor(private router: Router) {}
  
    ngOnInit(): void {
      this.buildForm();
    }
  
    private buildForm(): void {
      // Create each interest control as a FormControl<boolean>
      const interestsArray = this.interests.map(
        () => new FormControl<boolean>(false, { nonNullable: true })
      );
  
      // Build the typed FormGroup
      this.onboardingForm = new FormGroup<OnboardingFormControls>({
        childAge: new FormControl<number | null>(null, {
          nonNullable: false  // allow null
        }),
        childGender: new FormControl<string>('', { nonNullable: true }),
        childInterests: new FormArray<FormControl<boolean>>(interestsArray),
        preferredDuration: new FormControl<string>('', { nonNullable: true })
      });
    }
  
    /** A convenient getter for the array of boolean controls. */
    get childInterests(): FormArray<FormControl<boolean>> {
      return this.onboardingForm.controls.childInterests;
    }
  
    // For the horizontal slider: 
    get translateXStyle(): string {
      return `translateX(${-this.currentIndex * 100}%)`;
    }
  
    // The conic-gradient ring behind the arrow button
    get progressCircleBackground(): string {
        const fraction = (this.currentIndex + 1) / this.slides.length;
        const degrees = fraction * 360;
        // For example, fraction=0.25 => degrees=90, so 25% pink, 75% gray
        // You want pink from 0deg -> degrees deg, gray from degrees deg -> 360
      
        return `conic-gradient(#a481ff 0deg, #a481ff ${degrees}deg, #fff ${degrees}deg, #fff 360deg)`;
      }
      
  
    // pointer down
    onPointerDown(event: PointerEvent) {
      this.isDragging = true;
      this.animating = false;
      this.startX = event.clientX;
      this.currentX = event.clientX;
    }
  
    // pointer move
    onPointerMove(event: PointerEvent) {
      if (!this.isDragging) return;
      this.currentX = event.clientX;
      const dx = this.currentX - this.startX;
      const wrapper = this.slidesWrapper.nativeElement;
      const offset = -this.currentIndex * wrapper.clientWidth + dx;
      wrapper.style.transform = `translateX(${offset}px)`;
      wrapper.style.transition = 'none';
    }
  
    // pointer up/cancel
    onPointerUp(event: PointerEvent) {
      if (!this.isDragging) return;
      this.isDragging = false;
      this.animating = true;
  
      const dx = this.currentX - this.startX;
      if (Math.abs(dx) > this.threshold) {
        // left swipe => next slide
        if (dx < 0 && this.currentIndex < this.slides.length - 1) {
          this.currentIndex++;
        }
        // right swipe => prev slide
        if (dx > 0 && this.currentIndex > 0) {
          this.currentIndex--;
        }
      }
      this.updateSlidePosition();
    }
  
    private updateSlidePosition() {
      const wrapper = this.slidesWrapper.nativeElement;
      wrapper.style.transition = 'transform 0.3s ease';
      wrapper.style.transform = this.translateXStyle;
    }
  
    // Called by the pink arrow button
    onNextClick() {
      if (this.currentIndex < this.slides.length - 1) {
        this.currentIndex++;
        this.updateSlidePosition();
      } else {
        // last slide => submit final data
        this.submitOnboarding();
      }
    }
  
    private submitOnboarding() {
      // gather final data from typed form
      const { childAge, childGender, childInterests, preferredDuration } = this.onboardingForm.value;
      // childInterests is an array of booleans matching `this.interests`.
  
      // Example: convert them to an array of strings
      const selectedInterests = this.childInterests.value
        .map((checked, i) => (checked ? this.interests[i] : null))
        .filter((v): v is string => v !== null);
  
      const finalData = {
        childAge,
        childGender,
        selectedInterests,
        preferredDuration
      };
  
      console.log('Onboarding data:', finalData);
  
      // Here you would post to your backend, e.g. /api/auth/onboarding
      // once done => navigate:
      this.router.navigate(['/home']);
    }
  }
  