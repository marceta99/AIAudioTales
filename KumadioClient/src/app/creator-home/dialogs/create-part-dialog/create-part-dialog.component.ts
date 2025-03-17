import { CommonModule } from "@angular/common";
import { Component, OnInit } from "@angular/core";
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { IDialogComponent } from "src/app/common/components/dialog/base/idialog-component.interface";


export interface CreatePartDialogProps { }

export interface CreatePartDialogResult {
  file: File;
  answers: string[];
}

@Component({
  selector: 'app-create-part-dialog',
  templateUrl: 'create-part-dialog.component.html',
  styleUrls: ['create-part-dialog.component.scss'],
  imports: [CommonModule, ReactiveFormsModule]
})
export class CreatePartDialogComponent implements IDialogComponent<CreatePartDialogProps, CreatePartDialogResult> ,OnInit {
  public partForm!: FormGroup;

  constructor(private formBuilder: FormBuilder) { }

  ngOnInit(): void {
    this.partForm = this.formBuilder.group({
          partAudio: ['', [Validators.required, Validators.maxLength(500)]],
          answers: this.formBuilder.array([])
    }) 
  }

  get partFormAnswers(): FormArray {
    return this.partForm.get('answers') as FormArray; // getter which returns FormArray of answers
  }

  public onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    
    if (!input.files || input.files.length === 0) {
      return; // no files selected
    }
  
    const file = input.files[0];
    
    // Patch the file object into the form control
    this.partForm.patchValue({
      partAudio: file
    });
  
    // If you want to trigger validation or valueChanges logic:
    this.partForm.get('partAudio')?.updateValueAndValidity();
  }

  public addPart(){
    const result: CreatePartDialogResult = {
      file: this.partForm.value.partAudio,
      answers: this.partFormAnswers.controls.map(control => control.value)
    };

    this.closeDialog(result);
  }

 public addAnswer(){
  if(this.partFormAnswers.length < 3){
    this.partFormAnswers.push(this.formBuilder.group({
      text: ['', Validators.maxLength(40)]
    }))
  }
  }

  public removeAnswer(index: number){
    this.partFormAnswers.removeAt(index);
  }

  public onOverlayClick(event: MouseEvent): void {
    // If the user clicks directly on the overlay (not inside the .dialog-container),
    // close the dialog.
    const target = event.target as HTMLElement;
    if (target.classList.contains('dialog-overlay')) {
      this.closeDialog();
    }
  }

  //region IDialogComponent Implementation

  public closeDialog!: (value?: CreatePartDialogResult | undefined) => void;
  public dialogProps!: CreatePartDialogProps;

  //#endregion
}