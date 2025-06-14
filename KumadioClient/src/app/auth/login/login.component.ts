import { Component, OnInit, NgZone, AfterViewInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { environment } from 'src/environments/environment';
import { Role } from 'src/app/entities';
import { AuthService } from '../services/auth.service';

 declare const google: any;
@Component({
  selector: 'app-login',
  imports: [
    CommonModule,
    RouterModule,        // for [routerLink] and Router
    ReactiveFormsModule  // for FormGroup, FormControl
  ],
  templateUrl: 'login.component.html',
  styleUrls: ['login.component.scss'],
})
export class LoginComponent implements OnInit {
  private clientId = environment.clientId;
  public showErrorMessage = false;
  public showGoogleErrorMessage = false;
  public showinvalidLoginMethod = false;
  public loginForm = new FormGroup({
      email: new FormControl("", { nonNullable: true, validators: [ Validators.required, Validators.email ] }),
      password: new FormControl("", { nonNullable: true, validators: [ Validators.required, Validators.minLength(6)] } )
  });

  constructor(
    private router: Router,
    private _ngZone: NgZone,
    private service: AuthService
  ) {}

  ngOnInit(): void {
    this.initializeGoogleSignIn();

  }

  public login() {
    const email = this.loginForm.controls['email'].value;
    const password = this.loginForm.controls['password'].value;
    this.service.login(email, password).subscribe({
      next: user => {
        user.role === Role.CREATOR
         ? this.router.navigate(['/creator'])
         : user.isOnboarded ? this.router.navigate(['/home']) : this.router.navigate(['/onboarding']); 
      },
      error: (error: any) => {
        console.log(error);
        this.loginForm.reset();
        this.showGoogleErrorMessage = false;

        if (error.error?.code === 'INVALID_LOGIN_METHOD') {
          this.showinvalidLoginMethod = true;
        } else {
          this.showErrorMessage = true;
        }
      }
    });
  }

  private initializeGoogleSignIn() {
    google.accounts.id.initialize({
      client_id: this.clientId,
      callback: (response: any) => this.handleCredentialResponse(response)
    });

    google.accounts.id.renderButton(
      document.getElementById('google-signin-button'),
      { theme: 'outline', size: 'large' }
    );

    google.accounts.id.prompt(); // also display the One Tap dialog
  }

  private handleCredentialResponse(response: any) {
    console.log('Encoded JWT ID token: ' + response.credential);
    this._ngZone.run(() => {
      this.service.loginWithGoogle(response.credential).subscribe({
      next: user => {
        user.role === Role.CREATOR
         ? this.router.navigate(['/creator'])
         : user.isOnboarded ? this.router.navigate(['/home']) : this.router.navigate(['/onboarding']); 
      },
      error: (error: any) => {
        console.log(error);
        this.loginForm.reset();
        this.showGoogleErrorMessage = true;
        this.showErrorMessage = false;
        this.showinvalidLoginMethod = false;
      }
    });
    });
  }
}
