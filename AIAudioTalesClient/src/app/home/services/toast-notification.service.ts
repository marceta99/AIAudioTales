import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import { Toast } from "src/app/entities";

@Injectable({
  providedIn: 'root'
})
export class ToastNotificationService {
  private toastSubject = new Subject<Toast>();
  toast$: Observable<Toast> = this.toastSubject.asObservable();

  show(toast: Toast): void {
    this.toastSubject.next(toast);
  }

}
