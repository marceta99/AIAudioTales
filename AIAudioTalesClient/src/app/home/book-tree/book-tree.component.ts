import { Component, OnInit, ViewEncapsulation } from '@angular/core';
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
export class BookTreeComponent implements OnInit{
  partForm!: FormGroup;
  bookId!: number;
  hasParts: boolean = false;
  partTree!: PartTree;
  isDialogActive: boolean = false;
  
  constructor(
    private spinnerService: LoadingSpinnerService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private bookService: BookService
    ) {}
  
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
      },
      error: (error: any) => {
        console.error('Error creating book:', error);
      }
    });
  }

  generateTreeHtml(part: PartTree): string {
    let html = `<li><a href="#"><span>${part.partName}</span></a>`;
    if (part.nextParts && part.nextParts.length > 0) {
      html += '<ul>';
      
      // answers that does not have next part
      part.answers.forEach(answer => {
        if(!answer.nextPartId){
          html += `<li><a href="#" class="not-added-part"><span>${answer.text}</span></a>`;
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
        html += `<li><a href="#" class="not-added-part"><span>${answer.text}</span></a>`;
      });
      html += '</ul>';
    }
    html += '</li>';
    return html;
  }

  onSubmit(){
    /*const answers = this.answers.controls.map(control => control.value);
    const createAnswers: CreateAnswer[] = answers.map(answer => ({
      text: answer
    }));
 
    if(this.partTree){ // if partTree is null this means that book does not have any parts so far so root part should be created first
      this.addRootPart(createAnswers);
    }else{
      this.addPart(createAnswers,parentAnswerId)
    }*/
  }

  private addRootPart(answers : CreateAnswer[]){
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
