// src/app/components/email-confirmation-sent/email-confirmation-sent.component.ts
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-email-confirmation-sent',
  templateUrl: './email-confirmation-sent.component.html',
  styleUrls: ['./email-confirmation-sent.component.scss'],
  imports: [                          
      CommonModule,
      ReactiveFormsModule,
      RouterModule
    ]
})
export class EmailConfirmationSentComponent implements OnInit {
  public email: string | null = null;
  
  constructor(private router: Router) {}

  ngOnInit() {
     this.email = this.router.getCurrentNavigation()?.extras.state?.['email'] ?? null;
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}
