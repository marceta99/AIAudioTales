import { Component, OnInit } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss']
})
export class StatisticsComponent implements OnInit{
  constructor(private spinnerService: LoadingSpinnerService) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
  }
}
