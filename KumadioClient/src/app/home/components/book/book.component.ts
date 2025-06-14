import { Component, OnInit } from '@angular/core';
import { DtoBookPreview, ReturnBook, Toast, ToastIcon, ToastType } from 'src/app/entities';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule, Location } from '@angular/common';
import { ToastNotificationService } from 'src/app/common/services/toast-notification.service';
import { CatalogService } from '../../services/catalog.service';
import { LibraryService } from '../../services/library.service';
import { BookCategoryPipe } from '../../pipes/category.pipe';
import { PlayerService } from '../../services/player.service';
import { switchMap } from 'rxjs/operators';

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
    private playerService: PlayerService,
    private route: ActivatedRoute,
    private router: Router,
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

  public goBack(): void {
    this.location.back(); // Go back to the previous route
  }

  public hasPreviousRoute(): boolean {
    return !!window.history.state.navigationId; // Check if there is a previous route
  }

  public addToLibrary(): void {
  this.libraryService.addToLibrary(this.book.id)
    .pipe(
      // as soon as addToLibrary succeeds, reload the full list
      switchMap(() => this.libraryService.getPurchasedBooks())
    )
    .subscribe({
      next: books => {
        this.userHasBook = true;

        // 1) overwrite the shared stream
        this.libraryService.purchasedBooks.next(books);

        // 2) fire your toast
        this.notificationService.show({
          text: "Added to library",
          toastIcon: ToastIcon.Success,
          toastType: ToastType.Success
        });
      },
      error: err => {
        console.error(err);
        this.notificationService.show({
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error
        });
      }
    });
}
  public togglePreview(): void {
    if (this.isPlaying) {
      this.audio.pause();
    } else {
      this.audio.play().catch(err => console.error('Audio play failed:', err));
    }
    this.isPlaying = !this.isPlaying;
  }

  public onListenClick(): void {
  // 1) Fetch the up-to-date list of purchased books
  this.libraryService.getPurchasedBooks().subscribe({
    next: (books) => {
      // 2) Push that new list into the shared Subject so PlayerComponent sees it
      this.libraryService.purchasedBooks.next(books);

      // 3) Find our book’s index in that array
      const idx = books.findIndex(b => b.bookId === this.book.id);
      if (idx >= 0) {
        // 4) Tell the player service to switch to that book
        this.playerService.currentBookIndex.next(idx);
      }

      // 5) Finally, navigate to whatever route your <app-player> is on
      this.router.navigate(['/home/library']);  
    },
    error: err => {
      console.error('Could not load library', err);
    }
  });
}

}
