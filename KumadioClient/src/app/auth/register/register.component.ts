import { Component, NgZone, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RegisterUser } from 'src/app/entities';
import { passwordMatchValidator } from '../custom-validators';
import { AuthService } from '../services/auth.service';

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
  public registerForm!: FormGroup;

  constructor(
    private authService: AuthService,
    private router: Router,
    private _ngZone: NgZone
  ) {}

  ngOnInit(): void {
    this.registerForm = new FormGroup({
      firstName: new FormControl(null, [Validators.required, Validators.minLength(2)]),
      lastName: new FormControl(null, [Validators.required, Validators.minLength(2)]),
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required, Validators.minLength(6)]),
      confirmPassword: new FormControl(null, [Validators.required, Validators.minLength(6)])
    }, {
      validators: passwordMatchValidator
    });
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
          this.router.navigate(['/login']).then(() => window.location.reload());
        });
      },
      error: (error: any) => {
        console.log(error);
        // handle error, e.g., reset form or show alert
      }
    });
  }

}
