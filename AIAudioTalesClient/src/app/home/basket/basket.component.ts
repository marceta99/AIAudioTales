import { Component, OnInit } from '@angular/core';
import { Basket, BasketItem } from 'src/app/entities';
import { BookService } from '../services/book.service';
import { LoadingSpinnerService } from '../services/loading-spinner.service';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss']
})
export class BasketComponent implements OnInit{
  basket!: Basket;
  constructor(private bookService : BookService, private spinnerService: LoadingSpinnerService) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(true);

    this.bookService.getBasket().subscribe({
      next: (basket: Basket) => {
        this.basket = basket;
        this.spinnerService.setLoading(false);
    },
      error: error => {
        this.spinnerService.setLoading(false);
        console.error('There was an error!', error);
    },

    })
  }

  removeBasketItem(itemId: number){
    this.spinnerService.setLoading(true);

    this.bookService.removeBasketItem(itemId).subscribe({
      next: (basket: Basket) => {
        this.basket = basket;
        this.spinnerService.setLoading(false);
    },
      error: error => {
        this.spinnerService.setLoading(false);
        console.error('There was an error!', error);
    },

    })
  }

}
