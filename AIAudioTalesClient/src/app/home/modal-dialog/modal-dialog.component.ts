import { Component, Input, TemplateRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-modal-dialog',
  templateUrl: './modal-dialog.component.html',
  styleUrls: ['./modal-dialog.component.scss']
})
export class ModalDialogComponent {
  @Input() title: string = 'Default Title';
  @Input() isActive: boolean = false;
  @ViewChild('content', { read: TemplateRef }) contentTemplate!: TemplateRef<any>;

  showModal() {
    this.isActive = true;
  }

  closeModal() {
    this.isActive = false;
  }
}
