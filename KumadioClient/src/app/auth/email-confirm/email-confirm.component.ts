import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-email-confirm',
  templateUrl: './email-confirm.component.html',
  styleUrls: ['./email-confirm.component.scss'],
  imports: [                          
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ]
})
export class ConfirmEmailComponent implements OnInit {
  isSuccess = false;
  isError = false;

  constructor(
    private route: ActivatedRoute,
    private auth: AuthService,
    private spinnerService: LoadingSpinnerService,
    private router: Router
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token') || '';
    this.spinnerService.setLoading(true);

    this.auth.confirmEmail(token).subscribe({
      next: () => {
        // isključi spinner, prikaži poruku o uspehu
        this.spinnerService.setLoading(false);
        this.isSuccess = true;
      },
      error: err => {
        // isključi spinner, prikaži grešku
        this.spinnerService.setLoading(false);
        this.isError = true;
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }

  retry(): void {
    this.router.navigate(['/confirm-email-sent']);
  }
}
