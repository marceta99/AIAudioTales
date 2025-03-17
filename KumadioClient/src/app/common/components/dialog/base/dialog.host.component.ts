import { Component, ViewChild, ViewContainerRef } from '@angular/core';
import { DialogService } from './dialog.service';

@Component({
  selector: 'app-dialog-host',
  template: `
    <!-- This <ng-template> is where we dynamically insert the modal -->
    <ng-template #dialogContainer></ng-template>
  `
})
export class DialogHostComponent {
  @ViewChild('dialogContainer', { read: ViewContainerRef, static: true }) public dialogContainerRef!: ViewContainerRef;

  constructor(private dialogService: DialogService) {}

  ngAfterViewInit() {
    // Register the container with the service
    this.dialogService.registerHost(this.dialogContainerRef);
  }
}
