import { Component, OnInit } from '@angular/core';
import { LoadingSpinnerService } from 'src/app/common/services/loading-spinner.service';

@Component({
  selector: 'app-analytics',
  templateUrl: 'analytics.component.html',
  styleUrls: ['analytics.component.scss']
})
export class AnalyticsComponent implements OnInit{
  constructor(private spinnerService: LoadingSpinnerService) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
  }
}
