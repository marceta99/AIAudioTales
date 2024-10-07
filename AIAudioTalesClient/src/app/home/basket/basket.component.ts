import { Component, OnInit } from '@angular/core';
import { Basket, BasketItem, Toast, ToastIcon, ToastType } from 'src/app/entities';
import { BookService } from '../services/book.service';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { Stripe, loadStripe } from '@stripe/stripe-js';
import { environment } from 'src/environment/environment';
import { StripeService } from '../services/stripe.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastNotificationService } from '../services/toast-notification.service';

@Component({
  selector: 'app-basket',
  templateUrl: './basket.component.html',
  styleUrls: ['./basket.component.scss']
})
export class BasketComponent implements OnInit{
  private stripePromise!: Promise<Stripe | null>;
  basket!: Basket;

  constructor(
     private bookService : BookService,
     private spinnerService: LoadingSpinnerService,
     private stripeService: StripeService, 
     private activatedRoute: ActivatedRoute,
     private notificationService: ToastNotificationService,
     private router: Router
     ) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
    this.bookService.basket.subscribe((basket: Basket)=>{
      this.basket = basket;
    })
    this.activatedRoute.fragment.subscribe(fragment => {
      if(fragment === "error"){
        const toast: Toast = {
          text: "We're sorry! An error occurred. Please try again later.",
          toastIcon: ToastIcon.Error,
          toastType: ToastType.Error
        }
        this.notificationService.show(toast);
      }
    });
  }

  removeBasketItem(itemId: number){
    this.spinnerService.setLoading(true);

    this.bookService.removeBasketItem(itemId).subscribe({
      next: (basket: Basket) => {
        this.bookService.basket.next(basket)
        this.basket = basket;
        this.spinnerService.setLoading(false);
    },
      error: error => {
        this.spinnerService.setLoading(false);
        console.error('There was an error!', error);
    },

    })
  }

  async checkout(){
    await this.pay();
  }

  async pay(){
    this.spinnerService.setLoading(true);

    this.stripePromise = loadStripe(environment.stripePublicKey);
    const stripe = await this.stripePromise;

    this.stripeService.createOrder(this.basket).subscribe({
      next: (sessionId: {sessionId: string}) => {
        console.log(sessionId)
        this.spinnerService.setLoading(false);
        stripe?.redirectToCheckout(sessionId);
    },
      error: error => {
        this.spinnerService.setLoading(false);
        console.error('There was an error!', error);
    },

    })
  }

  navigateToBook(bookId: number){
    this.router.navigate(['/home/books',bookId])
  }
}
