import { Component, OnInit } from '@angular/core';
import { Basket, ReturnBook, User } from 'src/app/entities';
import { AuthService } from 'src/app/auth/services/auth.service';
import { animate, style, transition, trigger } from '@angular/animations';
import { CommonModule, Location } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, filter, Observable, switchMap } from 'rxjs';
import { SearchService } from '../../services/search.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss'],
  animations: [
    trigger('slideInOut', [
      transition(':enter', [
        style({ transform: 'translateX(100%)', opacity: 0 }), // Start position: off-screen to the right
        animate('300ms ease-in-out', style({ transform: 'translateX(0)', opacity: 1 })), // Slide in
      ]),
      transition(':leave', [
        animate('300ms ease-in-out', style({ transform: 'translateX(100%)', opacity: 0 })) // Slide out
      ])
    ]), 
    trigger('slideUpDownLogo', [
      transition(':enter', [
        style({ transform: 'translateY(-100%)', opacity: 0 }), // Start above the screen
        animate('300ms ease-in-out', style({ transform: 'translateY(0)', opacity: 1 })) // Slide down
      ]),
      transition(':leave', [
        animate('300ms ease-in-out', style({ transform: 'translateY(-100%)', opacity: 0 })) // Slide up
      ])
    ]),
    trigger('slideInIcons', [
      transition(':enter', [
        style({ transform: 'translateX(-100%)', opacity: 0 }),
        animate('500ms ease-in-out', style({ transform: 'translateX(0)', opacity: 1 })), // Slide in
      ])
    ]),
    trigger('slideDownUp', [
      transition(':enter', [
        style({ transform: 'translateY(100%)', opacity: 0 }),
        animate('300ms ease-in-out', style({ transform: 'translateY(-40%)', opacity: 1 }))
      ]),
      transition(':leave', [
        animate('300ms ease-in-out', style({ transform: 'translateY(-100%)', opacity: 0 })) // Slide up
      ])
    ]),

  ],
  imports: [
      CommonModule,
      RouterModule,        
      ReactiveFormsModule,
  ]
})
export class SidenavComponent implements OnInit {
  public searchControl = new FormControl();
  public isSearchActive$!: Observable<boolean>;

  constructor(
    private searchService: SearchService,
    private authService: AuthService,
    private location: Location
  ) {}

  ngOnInit(): void {
    this.isSearchActive$ = this.searchService.isSearchActive$;

    this.searchControl.valueChanges
    .pipe(
      debounceTime(300),                                                                            // Wait for 300ms pause in events
      distinctUntilChanged(),                                                                       // Only emit if value is different from previous value
      filter((searchTerm: string) => {                                                              // search term is empty then show search history and if search term is empty string or just empty white space return false so that http request to the backend is not send
        const searchIsEmpty = searchTerm.trim().length === 0;
        if (searchIsEmpty) {
          return false;
        } else {
          this.searchService.searchHistory.next([]);
          return true;
        }                                         
      }),
      switchMap((searchTerm : string) => {
        this.searchService.searchTerm = searchTerm;                                                          
        return this.searchService.searchBooks(searchTerm,1,10)   // we use switch map because If a new term is entered while a previous search request is still in progress, that HTTP request is cancelled, and only the latest search is processed.
      })
    )
    .subscribe({
      next: (books : ReturnBook[]) => {
        console.log("searched books", books)
        this.searchService.searchedBooks.next(books);
      },
      error: error => {
        console.error('There was an error!', error);
      }
    })
  }

  public toggleSearch(): void {
    if (this.searchService.isSearchActive.value) {
      this.searchService.isSearchActive.next(false);
      this.location.back();
    } else {
      this.searchService.isSearchActive.next(true);
    }
  }
  
}
