import { ChangeDetectorRef, Component, ElementRef, OnInit, Renderer2, ViewChild, ViewEncapsulation } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ReturnPart, CreatePart, CreateRootPart, PartTree } from 'src/app/entities';
import { BookService } from '../services/book.service';
import { ModalDialogComponent } from '../modal-dialog/modal-dialog.component';

@Component({
  selector: 'app-book-tree',
  templateUrl: './book-tree.component.html',
  styleUrls: ['./book-tree.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class BookTreeComponent implements OnInit {
  rootPartForm!: FormGroup;
  partForm!: FormGroup;
  bookId!: number;
  partTree!: PartTree;
  clickedPartId!: number;
  clickedPart!: ReturnPart | undefined;
  selectedFile: File | null = null;
  audioLink: string | null = null;

  @ViewChild('treeContainer') treeContainer!: ElementRef;
  @ViewChild('answerModal', { static: false }) answerModal!: ModalDialogComponent;
  @ViewChild('partModal', { static: false }) partModal!: ModalDialogComponent;
  @ViewChild('audioPlayer') audioPlayer!: ElementRef<HTMLAudioElement>;
  
  constructor(
    private spinnerService: LoadingSpinnerService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private bookService: BookService,
    private cdr: ChangeDetectorRef,
    private renderer: Renderer2
    ) {}
  
  ngOnInit(): void {
    this.spinnerService.setLoading(false);

    this.route.params.subscribe(params => {
      this.bookId = +params['bookId']; 

      this.getBookTree();      
    });

    this.rootPartForm = this.formBuilder.group({
      partAudio: ['', Validators.required],
      answers: this.formBuilder.array([])
    }) 

    this.partForm = this.formBuilder.group({
      partAudio: ['', [Validators.required, Validators.maxLength(500)]],
      answers: this.formBuilder.array([])
    }) 
  }

  public onFileChange(event: any) {
    this.selectedFile = event.target.files[0];
    console.log("selected file", this.selectedFile)
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

    spans.forEach(span => {
      const answerId = +span.getAttribute('data-answer-id')!; // Use + to convert to number
      const partId = +span.getAttribute('data-part-id')!; 
      this.renderer.listen(span, 'click', (event) => {

        if(answerId){ // this means that tree part with no added part is clicked
          this.clickedPartId = answerId;
          //here open answer modal
          this.answerModal.showModal();
        }else if(partId){ // this means that tree part with added part is added
          this.clickedPartId = partId;
          this.bookService.getPart(partId).subscribe({
            next: (part: ReturnPart) => {
              this.clickedPart = part;
              this.resetAudioPlayer();
              this.cdr.detectChanges(); 
              this.partModal.showModal();
              console.log("clicked part", this.clickedPart)
            },
            error: (error: any) => {
              console.error('Error creating book:', error);
            }
          });
        } 
      });
    });
  }

  public resetAudioPlayer() {
    if (this.audioPlayer) {
      this.audioPlayer.nativeElement.pause();
      this.audioPlayer.nativeElement.currentTime = 0;
      this.audioPlayer.nativeElement.load(); // Reload the audio element
    }
  }

  private generateTreeHtml(part: PartTree): string {
    let html = `<li><span class="tree-part" data-part-id=${part.partId}><span>${part.partName}</span></span>`;
    if (part.nextParts && part.nextParts.length > 0) {
      html += '<ul>';
      
      // answers that does not have next part
      part.answers.forEach(answer => {
        if(!answer.nextPartId){
          html += `<li><span class="tree-part not-added-part" data-answer-id="${answer.id}"><span>${answer.text}</span><span class="tooltip">Not Added Part Audio</span></span>`;
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
        html += `<li><span class="tree-part not-added-part" data-answer-id="${answer.id}"><span>${answer.text}</span><span class="tooltip">Not Added Part Audio</span></span>`;
      });
      html += '</ul>';
    }
    html += '</li>';
    return html;
  }

  public addRootPart(){
    const answers = this.rootPartFormAnswers.controls.map(control => control.value);

    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('bookId', this.bookId.toString());
      formData.append('partAudio', this.selectedFile);   
      formData.append('answers', JSON.stringify(answers));     
  
      this.bookService.addRootPart(formData).subscribe({
        next: (rootPart: ReturnPart) => {
          console.log('rootPart created successfully', rootPart);
          this.getBookTree();
        },
        error: (error) => {
          this.answerModal.closeModal();
          console.error('Error creating root part:', error);
        }
      });
      
      this.selectedFile = null;
    }
  }

  public addPart(){
   const answers = this.partFormAnswers.controls.map(control => control.value);

    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('bookId', this.bookId.toString());
      formData.append('partAudio', this.selectedFile);
      formData.append('answers', JSON.stringify(answers)); 
      formData.append('parentAnswerId', this.clickedPartId.toString());

      this.bookService.addPart(formData).subscribe({
        next: (newPart: ReturnPart) => {
          console.log('part created successfully', newPart);
          this.answerModal.closeModal();
          this.partForm.reset();
          this.getBookTree();
          // here I want to empty answer list because previously I was getting list from last submit
        },
        error: (error) => {
          this.answerModal.closeModal();
          this.partForm.reset();
           // here I want to empty answer list because previously I was getting list from last submit
          console.error('Error creating new part:', error);
        }
      });
    }

    
  }

  get rootPartFormAnswers(): FormArray {
    return this.rootPartForm.get('answers') as FormArray; // getter which returns FormArray of answers
  }

  get partFormAnswers(): FormArray {
    return this.partForm.get('answers') as FormArray; // getter which returns FormArray of answers
  }

  public addAnswer(answers: FormArray){
    if(answers.length < 3){
      answers.push(this.formBuilder.group({
        text: ['', Validators.maxLength(40)]
      }))
    }
  }

  public removeAnswer(answers: FormArray, index: number){
    answers.removeAt(index);
  }

}
