import { Component, OnInit, AfterViewInit, ElementRef, ViewChild, QueryList, ViewChildren } from '@angular/core';
import { AuthService } from 'src/app/auth/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastNotificationService } from '../services/toast-notification.service';
import { LoadingSpinnerService } from '../services/loading-spinner.service';
import { User } from 'src/app/entities';

@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.scss']
})
export class MyProfileComponent implements OnInit, AfterViewInit {
  currentUser!: User | null;
  isDarkTheme = false; // Toggle for theme
  selectedLanguage = 'English'; // Default language

  constructor(
    private authService: AuthService,
    private activatedRoute: ActivatedRoute,
    private notificationService: ToastNotificationService,
    private spinnerService: LoadingSpinnerService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
    this.authService.currentUser.subscribe(user => {
      this.currentUser = user;
    });
  }

  ngAfterViewInit(): void {}

  toggleTheme(): void {
    this.isDarkTheme = !this.isDarkTheme;
  }

  signOut(): void {
    this.router.navigate(['/login']);
  }
}
