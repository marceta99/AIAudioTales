import { Component, OnInit } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Category, CreateBook } from 'src/app/entities';
import { BookService } from '../services/book.service';

@Component({
  selector: 'app-create-book',
  templateUrl: './create-book.component.html',
  styleUrls: ['./create-book.component.scss']
})
export class CreateBookComponent implements OnInit {
  bookForm!: FormGroup;
  bookCategories!: Category[];

  constructor(
    private spinnerService: LoadingSpinnerService,
    private bookService: BookService, 
    private formBuilder: FormBuilder) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);

    this.bookService.getAllCategories().subscribe((categories: Category[])=>{
      this.bookCategories = categories;
    })

    this.bookForm = this.formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(40)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      imageURL: ['', [Validators.required, Validators.maxLength(500)]],
      categoryId: [null, [Validators.required]],
      partAudioLink: ['', [Validators.required, Validators.maxLength(500)]],
      answers: this.formBuilder.array([])
    })
  }

  onSubmit(){
    const answers = this.answers.controls.map(control => control.value);

    const newBook : CreateBook = {
      title : this.bookForm.controls['title'].value, 
      description : this.bookForm.controls['description'].value,
      imageURL : this.bookForm.controls['imageURL'].value, 
      categoryId : this.bookForm.controls['categoryId'].value, 
      price: 12,
      rootPart: {
        partAudioLink : this.bookForm.controls['partAudioLink'].value, 
        answers: answers
      }
    }

    this.bookService.createBook(newBook).subscribe({
      next: (response) => {
        console.log('Book created successfully', response);
      },
      error: (error) => {
        console.error('Error creating book:', error);
      }
    });
  }

  get answers(): FormArray {
    return this.bookForm.get('answers') as FormArray; // getter which returns FormArray of answers
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


