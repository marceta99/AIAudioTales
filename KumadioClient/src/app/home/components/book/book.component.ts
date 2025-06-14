import { Component, OnInit } from '@angular/core';
import { DtoBookPreview, ReturnBook, Toast, ToastIcon, ToastType } from 'src/app/entities';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule, Location } from '@angular/common';
import { ToastNotificationService } from 'src/app/common/services/toast-notification.service';
import { CatalogService } from '../../services/catalog.service';
import { LibraryService } from '../../services/library.service';
import { BookCategoryPipe } from '../../pipes/category.pipe';

@Component({
  selector: 'app-book',
  templateUrl: './book.component.html',
  styleUrls: ['./book.component.scss'],
  imports: [CommonModule, BookCategoryPipe, RouterModule]
})
export class BookComponent implements OnInit{
  public book!: DtoBookPreview;
  public userHasBook: boolean = false;
  public isPlaying = false;
  private audio = new Audio();

  constructor(
    private catalogService: CatalogService,
    private libraryService: LibraryService,
    private route: ActivatedRoute,
    private location: Location,
    private notificationService: ToastNotificationService) {}

  ngOnInit():void{
    this.route.params.subscribe(params => {
      const id = +params['bookId'];

      this.catalogService.getBookWithPreivew(id).subscribe({
        next: (book: DtoBookPreview) => {
          console.log(book);
          this.book = book;

          this.audio.src = this.book.rootPartAudio; 
          this.audio.load();
          this.audio.onended = () => {
            this.isPlaying = false;
          };

          this.libraryService.userHasBook(this.book.id).subscribe({
            next: (hasBook: boolean) => {
              this.userHasBook = hasBook;
            },
            error: (error: any) => {
              console.log(error);
            }
          });

        },
        error: (error: any) => {
          console.log(error);
        }
      });
    });
  }

  goBack(): void {
    this.location.back(); // Go back to the previous route
  }

  hasPreviousRoute(): boolean {
    return !!window.history.state.navigationId; // Check if there is a previous route
  }

  addToLibrary(): void { 
    this.libraryService.addToLibrary(this.book.id).subscribe({
      next: () => {
        this.userHasBook = true;
        const toast: Toast = {
          text: "Added to library",
          toastIcon: ToastIcon.Success,
          toastType: ToastType.Success
        }
        this.notificationService.show(toast);

    },
      error: (error : Error) => {
        console.log(error)
        const toast: Toast = {
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error
        }
        this.notificationService.show(toast);
    }
    })
  }

  public togglePreview(): void {
    if (this.isPlaying) {
      this.audio.pause();
    } else {
      this.audio.play().catch(err => console.error('Audio play failed:', err));
    }
    this.isPlaying = !this.isPlaying;
  }
}
