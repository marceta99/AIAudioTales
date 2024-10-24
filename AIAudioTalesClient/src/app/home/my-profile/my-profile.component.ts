import { AfterViewInit, Component, ElementRef, OnInit, QueryList, ViewChild, ViewChildren } from '@angular/core';
import { StripeService } from '../services/stripe.service';
import { Stripe, loadStripe } from '@stripe/stripe-js';
import { environment } from 'src/environment/environment';
import { Toast, ToastIcon, ToastType, User } from 'src/app/entities';
import { AuthService } from 'src/app/auth/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastNotificationService } from '../services/toast-notification.service';
import { LoadingSpinnerService } from '../services/loading-spinner.service';

@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.scss']
})
export class MyProfileComponent implements OnInit, AfterViewInit {
  private stripePromise!: Promise<Stripe | null>;
  currentUser!: User | null;

  @ViewChild('tabsContainer', { static: true }) tabsContainer!: ElementRef; // Reference to the tabs container
  @ViewChildren('tab') tabs!: QueryList<ElementRef>; // References to all tabs
  
  constructor(
    private stripeService: StripeService,
    private authService: AuthService, 
    private activatedRoute: ActivatedRoute,
    private notificationService: ToastNotificationService,
    private spinnerService: LoadingSpinnerService,
    private router: Router
    ) {}

  ngOnInit(): void {
   this.spinnerService.setLoading(false);
   this.authService.currentUser.subscribe(user=> {this.currentUser = user}) 

   this.activatedRoute.fragment.subscribe(fragment => {
    if(fragment === "error"){
      const toast: Toast = {
        text: "We're sorry! An error occurred. Please try again later.",
        toastIcon: ToastIcon.Error,
        toastType: ToastType.Error
      }
      this.notificationService.show(toast);
    }else if(fragment === "success") {
      this.authService.getCurrentUser().subscribe({
        next: (user: User) => {  
          console.log(user)
          this.authService.isLoggedIn = true;
          this.authService.currentUser.next(user);
        },
        error: (error) => {
          console.error('Error', error)
          this.router.navigate(['/login']).then(()=> window.location.reload());
        }
      });
      const toast: Toast = {
        text: "Successfully purchased subscription",
        toastIcon: ToastIcon.Success,
        toastType: ToastType.Success
      }
      this.notificationService.show(toast);
    }
   });
  }

  
  ngAfterViewInit(): void {
    this.scrollToActiveTab();
  }

  scrollToActiveTab(): void {
    const currentRoute = this.router.url.split('/').pop(); // Get the current route (e.g., 'account', 'language')
    let activeTabElement: ElementRef | undefined;

    // Find the corresponding tab for the active route
    this.tabs.forEach((tab) => {
      const tabRoute = tab.nativeElement.getAttribute('routerLink'); // Get the router link from the tab
      if (tabRoute === currentRoute) {
        activeTabElement = tab; // Match the tab to the current route
      }
    });

    // Scroll to the active tab if found
    if (activeTabElement) {
      activeTabElement.nativeElement.scrollIntoView({ inline: 'center', behavior: 'smooth' });
    }
  }

  async createSubscribeSession(){
    this.stripePromise = loadStripe(environment.stripePublicKey);
    const stripe = await this.stripePromise;
    
    this.stripeService.createSubscribeSession()
      .subscribe({
        next: (sessionId: {sessionId: string}) => {  
          stripe?.redirectToCheckout(sessionId);
        },
        error: (error) => console.error('Error creating checkout session', error),
      });
  }

  
}
