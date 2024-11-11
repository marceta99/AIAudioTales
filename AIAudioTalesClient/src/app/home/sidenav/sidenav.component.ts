import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { navbarData } from './nav-data';
import { RouterLinkActive } from '@angular/router';
import { BookService } from '../services/book.service';
import { Observable } from 'rxjs';
import { Basket, BasketItem, Book, User } from 'src/app/entities';
import { AuthService } from 'src/app/auth/services/auth.service';
import { animate, style, transition, trigger } from '@angular/animations';


export interface SideNavToggle{
  screenWidth: number;
  collapsed: boolean;
}

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
})
export class SidenavComponent implements OnInit {
  
  @Output() onToggleSideNav: EventEmitter<SideNavToggle> = new EventEmitter();
  screenWidth = 0;
  collapsed = false;
  navData = navbarData;
  itemsCount: number = 0;
  currentUser!: User | null;
  isSearchActive: boolean = false;

  constructor(private bookService: BookService, private authService: AuthService) {}

  ngOnInit(): void {
    this.bookService.basket.subscribe((basket: Basket)=>{
      this.itemsCount = basket.basketItems.length;
    })

    this.authService.currentUser.subscribe(user=> {this.currentUser = user});
  }

  toggleCollapse(): void{
    this.collapsed = !this.collapsed;
    this.onToggleSideNav.emit({collapsed: this.collapsed, screenWidth: this.screenWidth})
  }
  closesSidenav(): void{
    this.collapsed = false;
    this.onToggleSideNav.emit({collapsed: this.collapsed, screenWidth: this.screenWidth})
  }

  toggleSearch(): void {
    this.isSearchActive = !this.isSearchActive;
  }
  
}
