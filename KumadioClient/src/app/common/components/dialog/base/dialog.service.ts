import { Injectable, ViewContainerRef, ComponentRef, Type } from '@angular/core';
import { Subject, Observable } from 'rxjs';
import { IDialogComponent } from './idialog-component.interface';
import { ExtractDialogProps, ExtractDialogResult } from './dialog-type-helper';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  private hostViewContainerRef!: ViewContainerRef;
  private activeDialogRef: ComponentRef<any> | null = null;

  registerHost(containerRef: ViewContainerRef) {
    this.hostViewContainerRef = containerRef;
  }

  /**
   * Opens a dialog component (implementing IDialogComponent<TProps, TResult>)
   * and returns an Observable<TResult | undefined>.
   *
   * @param component The dialog component class
   * @param props The strongly typed props to pass to that dialog component
   */
  open<T extends IDialogComponent<any, any>>(
    component: Type<T>,
    props: ExtractDialogProps<T>
  ): Observable<ExtractDialogResult<T> | undefined> {
    // If you only allow one dialog at a time, close the existing one:
    this.close();

    // Dynamically create the component in the host container
    const compRef = this.hostViewContainerRef.createComponent(component);
    this.activeDialogRef = compRef;

    // We use a Subject to emit the final result once the dialog closes
    const result$ = new Subject<ExtractDialogResult<T> | undefined>();

    // Assign the dialog props to the component’s `dialogProps` property
    compRef.instance.dialogProps = props;

    // Overwrite the `closeDialog` method so it sends the result back
    compRef.instance.closeDialog = (value?: ExtractDialogResult<T>) => {
      result$.next(value);
      result$.complete();
      this.close(); // destroy the component
    };

    return result$.asObservable();
  }

  close() {
    if (this.activeDialogRef) {
      this.activeDialogRef.destroy();
      this.activeDialogRef = null;
    }
  }
}
