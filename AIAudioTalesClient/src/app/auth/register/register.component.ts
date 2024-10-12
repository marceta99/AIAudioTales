import { Component, NgZone, OnInit } from '@angular/core';
import { RegisterUser } from 'src/app/entities';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { passwordMatchValidator } from 'src/app/custom-validators';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {
  private user! : RegisterUser;
  registerForm!: FormGroup;

  constructor(private authService: AuthService, private router:Router, private _ngZone: NgZone) {}

  ngOnInit(): void {
    this.registerForm = new FormGroup({
      firstName: new FormControl(null, [Validators.required, Validators.minLength(2)]),
      lastName: new FormControl(null, [Validators.required, Validators.minLength(2)]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required, Validators.minLength(6)]),
      confirmPassword: new FormControl(null, [Validators.required, Validators.minLength(6)])
    }, {validators: passwordMatchValidator});
  }

  onSubmit() {
    this.user = {
      firstName: this.registerForm.controls['firstName'].value,
      lastName: this.registerForm.controls['lastName'].value,
      email: this.registerForm.controls['email'].value,
      password: this.registerForm.controls['password'].value,
      confirmPassword: this.registerForm.controls['confirmPassword'].value
    }

    if (this.user.password !== this.user.confirmPassword) {
    } else {
      // Passwords match, so proceed with registration
      this.registerUser(this.user);
    }
  }

  registerUser(user: RegisterUser) {
    this.authService.register(user).subscribe({
      next :(x:any) => {
        this._ngZone.run(()=>{
          this.router.navigate(['/login']).then(()=> window.location.reload());
        });
      },
      error :(error:any) => {
          console.log(error);
        }
  });
  }
}

