import { ChangeDetectorRef, Component, ElementRef, OnInit, Renderer2, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';
import { ReturnPart, PartTree } from 'src/app/entities';
import { CreatorService } from '../../services/creator.service';
import { CommonModule } from '@angular/common';
import { DialogService } from 'src/app/common/components/dialog/base/dialog.service';
import { filter, switchMap } from 'rxjs';
import { CreatePartDialogComponent } from '../../dialogs/create-part-dialog/create-part-dialog.component';
import { PartInfoDialogComponent } from '../../dialogs/part-info-dialog/part-info-dialog.component';


@Component({
  selector: 'app-book-tree',
  templateUrl: 'book-tree.component.html',
  styleUrls: ['book-tree.component.scss'],
  imports: [CommonModule, ReactiveFormsModule],
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
  
  constructor(
    private spinnerService: LoadingSpinnerService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private creatorService: CreatorService,
    private cdr: ChangeDetectorRef,
    private renderer: Renderer2,
    private dialogService: DialogService
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

  private getBookTree(): void {
    this.creatorService.getBookTree(this.bookId).subscribe({
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
          this.dialogService.open(CreatePartDialogComponent, {})
            .pipe(
              filter(result => !!result),
              switchMap(result => {
                const formData = new FormData();
                formData.append('bookId', this.bookId.toString());
                formData.append('partAudio', result.file);
                formData.append('answers', JSON.stringify(result.answers)); 
                formData.append('parentAnswerId', this.clickedPartId.toString());

                return this.creatorService.addBookPart(formData);
              }))
            .subscribe(newPart => {
                console.log('part created successfully', newPart);
                this.getBookTree();
                // here I want to empty answer list because previously I was getting list from last submit
              })

        }else if(partId){ // this means that tree part with added part is added
          this.clickedPartId = partId;
          this.creatorService.getPart(partId).subscribe((part: ReturnPart) => {
            this.clickedPart = part;
            this.cdr.detectChanges(); 
            this.dialogService.open(PartInfoDialogComponent, { clickedPart : this.clickedPart }).subscribe()
          })
        }
      })
    }); 
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
    const answers = this.rootPartFormAnswers.controls.map(control => control.value.text);

    if (this.selectedFile) {
      const formData = new FormData();
      formData.append('bookId', this.bookId.toString());
      formData.append('partAudio', this.selectedFile);   
      formData.append('answers', JSON.stringify(answers));     
  
      this.creatorService.addRootPart(formData).subscribe((rootPart: ReturnPart) => {
        console.log('rootPart created successfully', rootPart);
        this.getBookTree();
      });
      
      this.selectedFile = null;
    }
  }

  get rootPartFormAnswers(): FormArray {
    return this.rootPartForm.get('answers') as FormArray; // getter which returns FormArray of answers
  }

  public addAnswer(answers: FormArray){
    if(answers.length < 3){
      answers.push(this.formBuilder.group({
        text: ['', Validators.maxLength(40)]
      }))
    }
  }

  public onFileChange(event: Event) {
     const input = event.target as HTMLInputElement;
    
    if (!input.files || input.files.length === 0) {
      return; // no files selected
    }
  
    this.selectedFile = input.files[0];
    console.log("selected file", this.selectedFile)
  }

  public removeAnswer(answers: FormArray, index: number){
    answers.removeAt(index);
  }

}
