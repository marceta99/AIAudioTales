import { Component, NgZone } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { RegisterCreator } from 'src/app/entities';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';
import { passwordMatchValidator } from 'src/app/custom-validators';

@Component({
  selector: 'app-register-creator',
  templateUrl: './register-creator.component.html',
  styleUrls: ['./register-creator.component.scss']
})
export class RegisterCreatorComponent {
  private creator! : RegisterCreator;
  registerForm!: FormGroup;

  constructor(private authService: AuthService, private router:Router, private _ngZone: NgZone) {}

  ngOnInit(): void {
    this.registerForm = new FormGroup({
      email: new FormControl(null, [Validators.required, Validators.email]),
      password: new FormControl(null, [Validators.required, Validators.minLength(6)]),
      confirmPassword: new FormControl(null, [Validators.required, Validators.minLength(6)])
    }, {validators: passwordMatchValidator});
  }

  onSubmit() {
    this.creator = {
      email: this.registerForm.controls['email'].value,
      password: this.registerForm.controls['password'].value,
      confirmPassword: this.registerForm.controls['confirmPassword'].value
    }

    if (this.creator.password !== this.creator.confirmPassword) {
    } else {
      // Passwords match, so proceed with registration
      this.registerCreator(this.creator);
    }
  }

  registerCreator(creator: RegisterCreator) {
    this.authService.registerCreator(creator).subscribe({
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
