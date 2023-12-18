import { Component, NgZone } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from 'src/environment/environment';
import { AuthService } from '../services/auth.service';
import { CredentialResponse } from 'google-one-tap';
import { FormControl, FormGroup, NgForm, Validators } from '@angular/forms';
import { User } from 'src/app/entities';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  private clientId = environment.clientId;
  showErrorMessage = false;
  loginForm!: FormGroup;

  constructor(
    private router: Router,
    private _ngZone: NgZone,
    private service: AuthService
    ) {}

  ngOnInit():void{
      this.loginForm = new FormGroup({
        email: new FormControl(null, [Validators.required, Validators.email]),
        password: new FormControl(null, [Validators.required, Validators.minLength(6)])
      });

      // @ts-ignore
      window.onGoogleLibraryLoad = () => {
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
        google.accounts.id.prompt((notification: PromptMomentNotification) => {});
      };
  }

   handleCredentialResponse(response: CredentialResponse) {
    this.service.loginWithGoogle(response.credential).subscribe({
      next :(x:any) => {
        localStorage.setItem("user","mihailo marcetic");
        this._ngZone.run(() => {
          this.router.navigate(['/home']);
        })},
      error :(error:any) => {
          console.log(error);
        }
  });

   }

   login(){
    this.service.login(this.loginForm.controls['email'].value ,this.loginForm.controls['password'].value)
    .subscribe({
      next :(user:User) => {
          this.showErrorMessage=false;
          localStorage.setItem("user",JSON.stringify(user));
          this.router.navigate(['/home']);
      },
      error :(error:any) => {
          console.log(error);
          this.loginForm.reset();
          this.showErrorMessage = true;
        }
    });
  }
}
