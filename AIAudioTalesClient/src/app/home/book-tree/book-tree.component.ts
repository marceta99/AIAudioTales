import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { PartTree } from 'src/app/entities';
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

      this.bookService.getBookTree(this.bookId).subscribe({
        next: (partTree: PartTree) => {
          console.log('BookTree', partTree);
          this.partTree = partTree;
        },
        error: (error: any) => {
          console.error('Error creating book:', error);
        }
      });
    });

    this.partForm = this.formBuilder.group({
      partAudioLink: ['', [Validators.required, Validators.maxLength(500)]],
      answers: this.formBuilder.array([])
    }) 
  }

  generateTreeHtml(part: PartTree): string {
    let html = `<li><a href="#"><span>${part.partName}</span></a>`;
    if (part.nextParts && part.nextParts.length > 0) {
      html += '<ul>';
      part.nextParts.forEach(nextPart => {
        html += this.generateTreeHtml(nextPart);
      });
      html += '</ul>';
    }
    html += '</li>';
    return html;
  }

  onSubmit(){
    const answers = this.answers.controls.map(control => control.value);

    /*const newPart : = {
      
      partAudioLink : this.partForm.controls['partAudioLink'].value, 
      answers: answers
      
    }

    this.bookService.AddPart(newBook).subscribe({
      next: (response) => {
        console.log('Book created successfully', response);
      },
      error: (error) => {
        console.error('Error creating book:', error);
      }
    });*/
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
}
