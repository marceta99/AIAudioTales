import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { navbarData } from './nav-data';
import { RouterLinkActive } from '@angular/router';
import { BookService } from '../services/book.service';
import { Observable } from 'rxjs';
import { Basket, BasketItem, Book } from 'src/app/entities';


export interface SideNavToggle{
  screenWidth: number;
  collapsed: boolean;
}

@Component({
  selector: 'app-sidenav',
  templateUrl: './sidenav.component.html',
  styleUrls: ['./sidenav.component.scss']
})
export class SidenavComponent implements OnInit {
  
  @Output() onToggleSideNav: EventEmitter<SideNavToggle> = new EventEmitter();
  screenWidth = 0;
  collapsed = false;
  navData = navbarData;
  itemsCount: number = 0;

  constructor(private bookService: BookService) {}

  ngOnInit(): void {
    this.bookService.basket.subscribe((basket: Basket)=>{
      this.itemsCount = basket.basketItems.length;
    })
  }

  toggleCollapse(): void{
    this.collapsed = !this.collapsed;
    this.onToggleSideNav.emit({collapsed: this.collapsed, screenWidth: this.screenWidth})
  }
  closesSidenav(): void{
    this.collapsed = false;
    this.onToggleSideNav.emit({collapsed: this.collapsed, screenWidth: this.screenWidth})
  }
  
}
