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
import { OnboardingDataDto, OnboardingQuestionDto, OnboardingQuestionType } from '../entities';
import { OnboardingService } from './onboarding.service';
  
  @Component({
    selector: 'app-onboarding',
    templateUrl: 'onboarding.component.html',
    styleUrls: ['onboarding.component.scss'],
    imports: [CommonModule, ReactiveFormsModule],
    providers: [OnboardingService]
  })
  export class OnboardingComponent implements OnInit {
    @ViewChild('slidesWrapper') slidesWrapper!: ElementRef<HTMLDivElement>;

    onboardingForm!: FormGroup;
    questions: OnboardingQuestionDto[] = [];

    currentIndex = 0;
    animating = false;
  
    // pointer event tracking
    private startX = 0;
    private currentX = 0;
    private isDragging = false;
    private threshold = 50; // px threshold for swipe

    public OnboardingQuestionType = OnboardingQuestionType; // expose enum as property just so will be able to use this in template 
  
    constructor(private router: Router, private onboardingService: OnboardingService) {}
  
    ngOnInit(): void {
      this.onboardingService.getQuestions()
        .subscribe(questions =>{
          this.questions = questions;
          this.buildForm();
        });
        
    }
    
    private buildForm() {
      // Kreiramo prazan objekat u koji ćemo ubacivati FormControl-ove ili FormArray-e
      const group: Record<string, FormControl<any> | FormArray> = {};

      // Prođemo kroz svako pitanje i na osnovu tipa kreiramo odgovarajući kontroler
      this.questions.forEach(q => {
        switch (q.type) {
          case OnboardingQuestionType.NumberInput:
            // Za „number“ input: FormControl koji drži broj ili null
            group[q.key] = new FormControl<number | null>(null);
            break;

          case OnboardingQuestionType.SingleChoice:
            // Za „single choice“ (radio): FormControl koji drži ID izabrane opcije
            group[q.key] = new FormControl<number | null>(null);
            break;

          case OnboardingQuestionType.MultiChoice:
            // Za „multi choice“ (checkbox array):
            // - pravimo niz FormControl<boolean> iste dužine kao što ima opcija
            // - svaki kontroler će biti false (neoznačeno) po defaultu
            const controls = q.options!.map(() => new FormControl<boolean>(false));
            group[q.key] = new FormArray(controls);
            break;

          default:
            // (tehnički se ne bi desilo, ali radi type-safety-a)
            throw new Error(`Unknown question type: ${q.type}`);
        }
      });

      // Na kraju, grupu kontrolera pretvaramo u FormGroup
      this.onboardingForm = new FormGroup(group);
    }

    public getFormArray(key: string): FormArray<FormControl<boolean>> {
      return this.onboardingForm.get(key) as FormArray<FormControl<boolean>>;
    }
  
    // For the horizontal slider: 
    get translateXStyle(): string {
      return `translateX(${-this.currentIndex * 100}%)`;
    }
  
    // The conic-gradient ring behind the arrow button
    get progressCircleBackground(): string {
        const fraction = (this.currentIndex + 1) / this.questions.length;
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
        if (dx < 0 && this.currentIndex < this.questions.length - 1) {
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
  
    onNextClick() {
      if (this.currentIndex < this.questions.length - 1) {
        this.currentIndex++;
        this.updateSlidePosition();
      } else {
        // last slide => submit final data
        this.submitOnboarding();
      }
    }
  
    private submitOnboarding() {
      const dto: OnboardingDataDto = {
        childAge: this.onboardingForm.get('childAge')?.value ?? undefined,
        selectedOptions: []
      };

      // Iterate throught each question and if it is single or multiple choice add selection to selectedOptions array
      this.questions.forEach(q => {
        const key = q.key;
        const control = this.onboardingForm.get(key);

        if (q.type === OnboardingQuestionType.SingleChoice) {
          const selectedId = (control as FormControl<number | null>).value;
          if (selectedId != null) {
            dto.selectedOptions.push(selectedId);
          }
        } 
        else if (q.type === OnboardingQuestionType.MultiChoice) {
          const arr = (control as FormArray<FormControl<boolean>>).controls;
          arr.forEach((checkboxCtrl, index) => {
            // If checkbox is selected add it to selectedOptions 
            if (checkboxCtrl.value) {
              dto.selectedOptions.push(q.options[index].id);
            }
          });
        }
      });

      this.onboardingService.completeOnboarding(dto).subscribe(() => {
        this.router.navigate(['/home']);
      });
    }
}