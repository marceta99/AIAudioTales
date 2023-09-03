import { Component, NgZone } from '@angular/core';
import { RegisterUser } from 'src/app/entities';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  user : RegisterUser = {
    email: '',
    password: '',
    confirmPassword: ''
  };
  passwordsMismatch = false;

  constructor(private authService: AuthService, private router:Router, private _ngZone: NgZone) {}

  onSubmit() {
    if (this.user.password !== this.user.confirmPassword) {
      this.passwordsMismatch = true;
    } else {
      // Passwords match, so proceed with registration
      this.passwordsMismatch = false;
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

