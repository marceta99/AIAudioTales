import { Component, OnInit } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Category, CreateBook } from 'src/app/entities';
import { BookService } from '../services/book.service';
import { Router } from '@angular/router';

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
    private formBuilder: FormBuilder,
    private router: Router) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);

    this.bookService.getAllCategories().subscribe((categories: Category[])=>{
      this.bookCategories = categories;
    })

    this.bookForm = this.formBuilder.group({
      title: ['', [Validators.required, Validators.maxLength(40)]],
      description: ['', [Validators.required, Validators.maxLength(500)]],
      imageURL: ['', [Validators.required]],
      categoryId: [null, [Validators.required]],
    })
  }

  onSubmit(){
    const newBook : CreateBook = {
      title : this.bookForm.controls['title'].value, 
      description : this.bookForm.controls['description'].value,
      imageURL : this.bookForm.controls['imageURL'].value, 
      categoryId : this.bookForm.controls['categoryId'].value, 
      price: 12
    }

    this.bookService.createBook(newBook).subscribe({
      next: (bookId: number) => {
        console.log('Book created successfully with id : ', bookId);
        this.router.navigate(['/home/book-tree',bookId]);
      },
      error: (error) => {
        console.error('Error creating book:', error);
      }
    });
  }



}


