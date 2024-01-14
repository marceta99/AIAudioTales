import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { Toast } from 'src/app/entities';

@Component({
  selector: 'app-toast-notification',
  templateUrl: './toast-notification.component.html',
  styleUrls: ['./toast-notification.component.scss']
})
export class ToastNotificationComponent implements OnInit, AfterViewInit, OnDestroy{
  // Object containing details for different types of toasts
  toastDetails : any = {
    timer: 50000,
    success: {
        icon: 'fa-circle-check',
        text: 'Success: This is a success toast.',
    },
    error: {
        icon: 'fa-circle-xmark',
        text: 'Error: This is an error toast.',
    },
    warning: {
        icon: 'fa-triangle-exclamation',
        text: 'Warning: This is a warning toast.',
    },
    info: {
        icon: 'fa-circle-info',
        text: 'Info: This is an information toast.',
    }
  }
  toasts: Toast[] = [];

  notifications!: any;
  buttons!: any;

  constructor(private elRef: ElementRef) {}

  ngAfterViewInit(): void {

  }
  ngOnInit(): void {

  }

  ngOnDestroy(): void {
    // Clear timeouts when the component is destroyed
    this.toasts.forEach(toast => {
      if (toast.timeoutId) {
        clearTimeout(toast.timeoutId);
      }
    });
  }

  createToast(type: string): void {
    const { icon, text } = this.toastDetails[type];
    const newToast: Toast = { id: type, icon, text };
    this.toasts.push(newToast);

    // Set timeout to remove toast
    newToast.timeoutId = setTimeout(() => {
      this.removeToast(newToast);
    }, this.toastDetails.timer);
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
