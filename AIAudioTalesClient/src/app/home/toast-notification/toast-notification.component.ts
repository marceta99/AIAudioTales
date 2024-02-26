import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { Toast } from 'src/app/entities';
import { ToastNotificationService } from '../services/toast-notification.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-toast-notification',
  templateUrl: './toast-notification.component.html',
  styleUrls: ['./toast-notification.component.scss']
})
export class ToastNotificationComponent implements OnInit, AfterViewInit, OnDestroy{
  toasts: Toast[] = [];

  private toastSubscription!: Subscription;

  constructor(private notificationService: ToastNotificationService) {}

  ngAfterViewInit(): void {

  }
  ngOnInit(): void {
    this.toastSubscription = this.notificationService.toast$.subscribe(toast =>{
        this.createToast(toast);
    })
  }

  ngOnDestroy(): void {
    this.toastSubscription.unsubscribe();
    // Clear timeouts when the component is destroyed
    this.toasts.forEach(toast => {
      if (toast.timeoutId) {
        clearTimeout(toast.timeoutId);
      }
    });
  }

  createToast(newToast: Toast): void {
    this.toasts.push(newToast);

    // Set timeout to remove toast
    newToast.timeoutId = setTimeout(() => {
      this.removeToast(newToast);
    }, 5000);
  }

  removeToast(toast: Toast): void {
    // Clear the timeout
    if (toast.timeoutId) {
      clearTimeout(toast.timeoutId);
    }
    // Remove the toast from the array
    this.toasts = this.toasts.filter(t => t !== toast);
  }
}
