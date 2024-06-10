import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, Renderer2, ViewChild, ViewEncapsulation } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { BookPart, CreateAnswer, CreatePart, CreateRootPart, PartTree } from 'src/app/entities';
import { BookService } from '../services/book.service';

@Component({
  selector: 'app-book-tree',
  templateUrl: './book-tree.component.html',
  styleUrls: ['./book-tree.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BookTreeComponent implements OnInit, AfterViewInit{
  partForm!: FormGroup;
  bookId!: number;
  hasParts: boolean = false;
  partTree!: PartTree;
  isDialogActive: boolean = false;

  @ViewChild('treeContainer') treeContainer!: ElementRef;
  
  constructor(
    private spinnerService: LoadingSpinnerService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private bookService: BookService,
    private cdr: ChangeDetectorRef,
    private renderer: Renderer2
    ) {}
  ngAfterViewInit(): void {
    console.log("tree container",this.treeContainer)
  }
  
  ngOnInit(): void {
    this.spinnerService.setLoading(false);

    this.route.params.subscribe(params => {
      this.bookId = +params['bookId']; 

      this.getBookTree();      
    });

    this.partForm = this.formBuilder.group({
      partAudioLink: ['', [Validators.required, Validators.maxLength(500)]],
      answers: this.formBuilder.array([])
    }) 
  }

  private getBookTree(): void {
    this.bookService.getBookTree(this.bookId).subscribe({
      next: (partTree: PartTree) => {
        console.log('BookTree', partTree);
        this.partTree = partTree;
        this.cdr.detectChanges();
         console.log("tree container",this.treeContainer)
         this.treeContainer.nativeElement.innerHTML = this.generateTreeHtml(this.partTree);
        this.addClickListeners(this.treeContainer.nativeElement);

      },
      error: (error: any) => {
        console.error('Error creating book:', error);
      }
    });
  }

  private addClickListeners(container: HTMLElement): void {
    const spans = container.querySelectorAll('span.tree-part');
    console.log("spans", spans)
    spans.forEach(span => {
      this.renderer.listen(span, 'click', (event) => {
        console.log('Span clicked:', event);
      });
    });
  }

  generateTreeHtml(part: PartTree): string {
    let html = `<li><span class="tree-part"><span>${part.partName}</span></span>`;
    if (part.nextParts && part.nextParts.length > 0) {
      html += '<ul>';
      
      // answers that does not have next part
      part.answers.forEach(answer => {
        if(!answer.nextPartId){
          html += `<li><span class="tree-part not-added-part"><span>${answer.text}</span><span class="tooltip">Not Added Part Audio</span></span>`;
        }
      });

      // answers that have next part
      part.nextParts.forEach(nextPart => {
        html += this.generateTreeHtml(nextPart);
      });
      html += '</ul>';
    }else if(part.answers && part.answers.length > 0){
      html += '<ul>';
      part.answers.forEach(answer => {
        html += `<li><span class="tree-part not-added-part"><span>${answer.text}</span><span class="tooltip">Not Added Part Audio</span></span>`;
      });
      html += '</ul>';
    }
    html += '</li>';
    return html;
  }


  public addRootPart(){
    const answers = this.answers.controls.map(control => control.value);

    const rootPart : CreateRootPart = {
      bookId: this.bookId,
      partAudioLink : this.partForm.controls['partAudioLink'].value, 
      answers: answers
    }

    this.bookService.addRootPart(rootPart).subscribe({
      next: (rootPart: BookPart) => {
        console.log('rootPart created successfully', rootPart);
        this.getBookTree();
      },
      error: (error) => {
        console.error('Error creating root part:', error);
      }
    });
  }

  private addPart(answers: CreateAnswer[], parentAnswerId: number){
    const newPart : CreatePart = {
      bookId: this.bookId,
      partAudioLink : this.partForm.controls['partAudioLink'].value, 
      answers: answers,
      parentAnswerId: parentAnswerId
    }

    this.bookService.addRootPart(newPart).subscribe({
      next: (newPart: BookPart) => {
        console.log('part created successfully', newPart);
        this.getBookTree();
      },
      error: (error) => {
        console.error('Error creating new part:', error);
      }
    });
  }

  get answers(): FormArray {
    return this.partForm.get('answers') as FormArray; // getter which returns FormArray of answers
  }

  addAnswer(){
    if(this.answers.length < 3){
      this.answers.push(this.formBuilder.group({
        text: ['', Validators.maxLength(40)]
      }))
    }
  }

  removeAnswer(index: number){
    this.answers.removeAt(index);
  }

  private openDialog() {
    this.isDialogActive = true;
  }
}
