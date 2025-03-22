import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { IDialogComponent } from "src/app/common/components/dialog/base/idialog-component.interface";
import { ReturnPart } from "src/app/entities";

export interface PartInfoDialogProps {
  clickedPart: ReturnPart
}

export interface PartInfoDialogResult {
}

@Component({
  selector: 'app-part-info-dialog',
  templateUrl: 'part-info-dialog.component.html',
  styleUrls: ['part-info-dialog.component.scss'],
  imports: [CommonModule]
})
export class PartInfoDialogComponent implements IDialogComponent<PartInfoDialogProps, PartInfoDialogResult>{
  
  public onOverlayClick(event: MouseEvent): void {
    // If user clicks directly on the overlay (not inside .dialog-container), close
    const target = event.target as HTMLElement;
    if (target.classList.contains('dialog-overlay')) {
      this.closeDialog();
    }
  }  

  //region IDialogComponent Implementation
  
  public closeDialog!: (value?: PartInfoDialogResult | undefined) => void;
  public dialogProps!: PartInfoDialogProps;

  //#endregion
}