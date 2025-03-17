import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-creator-sidenav',
  templateUrl: 'creator-sidenav.component.html',
  styleUrls: ['creator-sidenav.component.scss'],
  imports: [
      CommonModule,
      RouterModule,        
      ReactiveFormsModule,
  ]
})
export class CreatorSidenavComponent {

}
