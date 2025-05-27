import { Component, OnInit, NgZone, AfterViewInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CredentialResponse, PromptMomentNotification } from 'google-one-tap';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { environment } from 'src/environments/environment';
import { Role } from 'src/app/entities';
import { AuthService } from '../services/auth.service';

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
export class LoginComponent implements OnInit, AfterViewInit {
  private clientId = environment.clientId;
  public showErrorMessage = false;
  public showGoogleErrorMessage = false;
  public loginForm!: FormGroup;

  constructor(
    private router: Router,
    private _ngZone: NgZone,
    private service: AuthService
  ) {}
  ngAfterViewInit(): void {
     // Initialize Google One Tap on window load
    // @ts-ignore
    window.onGoogleLibraryLoad = () => {
      console.log("test google ")
      // @ts-ignore
      google.accounts.id.initialize({
        client_id: this.clientId,
        callback: this.handleCredentialResponse.bind(this),
        auto_select: false,
        cancel_on_tap_outside: true
      });
      // @ts-ignore
      google.accounts.id.renderButton(
        // @ts-ignore
        document.getElementById("buttonDiv"),
        { theme: "outline", size: "large", width: "100%" }
      );
      // @ts-ignore
      google.accounts.id.prompt(
        (notification: PromptMomentNotification) => {}
      );
    };
  }

  ngOnInit(): void {
    this.loginForm = new FormGroup({
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required, Validators.minLength(6)])
    });
  }

  handleCredentialResponse(response: CredentialResponse) {
    /*this.service.loginWithGoogle(response.credential).subscribe({
      next: (user: User) => {
        // handle successful login
      },
      error: (error: any) => {
        console.log(error);
        this.loginForm.reset();
        this._ngZone.run(() => {
          this.showErrorMessage = false;
          this.showGoogleErrorMessage = true;
        });
      }
    });*/
  }

  login() {
    const email = this.loginForm.controls['email'].value;
    const password = this.loginForm.controls['password'].value;
    this.service.login(email, password).subscribe({
      next: user => {
        user.role === Role.CREATOR
         ? this.router.navigate(['/creator'])
         : true ? this.router.navigate(['/onboarding']) : this.router.navigate(['/home']);
      },
      error: (error: any) => {
        console.log(error);
        this.loginForm.reset();
        this.showGoogleErrorMessage = false;
        this.showErrorMessage = true;
      }
    });
  }
}
