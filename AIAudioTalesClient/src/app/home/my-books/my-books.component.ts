import { Component, OnInit } from '@angular/core';
import { LoadingSpinnerService } from '../services/loading-spinner.service';

@Component({
  selector: 'app-my-books',
  templateUrl: './my-books.component.html',
  styleUrls: ['./my-books.component.scss']
})
export class MyBooksComponent implements OnInit {
  constructor(private spinnerService: LoadingSpinnerService) {}

  ngOnInit(): void {
    this.spinnerService.setLoading(false);
  }
}
