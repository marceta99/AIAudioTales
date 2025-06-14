import { Component, NgZone, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterUser } from 'src/app/entities';
import { passwordMatchValidator } from '../custom-validators';
import { AuthService } from '../services/auth.service';
import { environment } from 'src/environments/environment';

declare const google: any;
@Component({
  selector: 'app-register',                  
  imports: [                          
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: 'register.component.html',
  styleUrls: ['register.component.scss']
})
export class RegisterComponent implements OnInit {
  private clientId = environment.clientId;
  public registerForm = new FormGroup({
    firstName: new FormControl('', { nonNullable: true, validators: [ Validators.required, Validators.minLength(2) ]}),
    lastName: new FormControl('',  { nonNullable: true, validators: [ Validators.required, Validators.minLength(2) ] }),
    email: new FormControl('',  { nonNullable: true, validators: [ Validators.required, Validators.email ]}),
    password: new FormControl('',  { nonNullable: true, validators: [ Validators.required, Validators.minLength(6) ] }),
    confirmPassword: new FormControl('', { nonNullable: true, validators: [ Validators.required, Validators.minLength(6) ] }),
  }, {
    validators: passwordMatchValidator
  });
  
  constructor(
    private authService: AuthService,
    private router: Router,
    private _ngZone: NgZone
  ) {}

  ngOnInit(): void {
    this.initializeGoogleRegistration();
    
  }

  public onSubmit(): void {
    const user: RegisterUser = {
      firstName: this.registerForm.controls['firstName'].value,
      lastName: this.registerForm.controls['lastName'].value,
      email: this.registerForm.controls['email'].value,
      password: this.registerForm.controls['password'].value,
      confirmPassword: this.registerForm.controls['confirmPassword'].value
    };

    this.authService.register(user).subscribe({
      next: () => {
        this._ngZone.run(() => {
          this.router.navigate(
            ['/email-confirmation-sent'],
            { state: { email: user.email } }
          );
        });
      },
      error: (error: any) => {
        console.log(error);
        this.registerForm.reset();
      }
    });
  }

  private initializeGoogleRegistration() {
    google.accounts.id.initialize({
      client_id: this.clientId,
      callback: (response: any) => this.handleCredentialResponse(response)
    });

    google.accounts.id.renderButton(
      document.getElementById('google-signin-button'),
      { theme: 'outline', size: 'large' }
    );

    google.accounts.id.prompt();
  }
  
  private handleCredentialResponse(response: any) {
    console.log('Encoded JWT ID token: ' + response.credential);
    this._ngZone.run(() => {
      this.authService.googleRegister(response.credential).subscribe({
      next: () => {
        this._ngZone.run(() => {
          this.router.navigate(['/login']).then(() => window.location.reload());
        });
      },
      error: (error: any) => {
        console.log(error);
        this.registerForm.reset();
      }
    });
    });
  }

}
