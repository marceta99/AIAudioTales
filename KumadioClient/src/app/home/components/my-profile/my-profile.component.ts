import { Component, OnInit, AfterViewInit } from '@angular/core';
import { AuthService } from 'src/app/auth/services/auth.service';
import { Router } from '@angular/router';
import { User } from 'src/app/entities';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-my-profile',
  templateUrl: 'my-profile.component.html',
  styleUrls: ['my-profile.component.scss'],
  imports: [CommonModule, FormsModule]
})
export class MyProfileComponent implements OnInit, AfterViewInit {
  currentUser$!: Observable<User | undefined>;
  isDarkTheme = false; // Toggle for theme
  selectedLanguage = 'English'; // Default language

  constructor(
    private authService: AuthService,
    private spinnerService: LoadingSpinnerService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
    this.currentUser$ = this.authService.currentUser$;
  }

  ngAfterViewInit(): void {}

  toggleTheme(): void {
    this.isDarkTheme = !this.isDarkTheme;
  }

  signOut(): void {
    this.router.navigate(['/login']);
  }
}
