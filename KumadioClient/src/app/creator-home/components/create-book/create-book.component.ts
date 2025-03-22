import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Book, Category, CreateBook } from 'src/app/entities';
import { Router } from '@angular/router';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';
import { CreatorService } from '../../services/creator.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-create-book',
  templateUrl: 'create-book.component.html',
  styleUrls: ['create-book.component.scss'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class CreateBookComponent implements OnInit {
  bookForm!: FormGroup;
  bookCategories!: Category[];

  constructor(
    private spinnerService: LoadingSpinnerService,
    private creatorService: CreatorService,
    private formBuilder: FormBuilder,
    private router: Router) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);

    this.creatorService.getAllCategories().subscribe((categories: Category[])=>{
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

    this.creatorService.createBook(newBook).subscribe({
      next: (book: Book) => {
        console.log('Book created successfully with id : ', book.id);
        this.router.navigate(['/creator/book-tree',book.id]);
      },
      error: (error) => {
        console.error('Error creating book:', error);
      }
    });
  }
}


