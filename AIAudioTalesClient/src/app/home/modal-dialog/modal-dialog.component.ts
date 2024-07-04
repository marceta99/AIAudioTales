// modal-dialog.component.ts
import { Component, Input, TemplateRef, AfterContentInit, ContentChild, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-modal-dialog',
  templateUrl: './modal-dialog.component.html',
  styleUrls: ['./modal-dialog.component.scss']
})
export class ModalDialogComponent implements AfterContentInit {
  @Input() title: string = 'Default Title';
  @Input() isActive: boolean = false;
  @ContentChild(TemplateRef) contentTemplate!: TemplateRef<any>;
  @Output() modalClosed: EventEmitter<void> = new EventEmitter<void>();

  showModal() {
    this.isActive = true;
  }

  closeModal() {
    this.isActive = false;
    this.modalClosed.emit();
  }

  ngAfterContentInit() {
    if (!this.contentTemplate) {
      console.error('No content template found');
    }
  }
}