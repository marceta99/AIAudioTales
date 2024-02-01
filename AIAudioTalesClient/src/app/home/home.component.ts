import { Component, OnInit } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { SideNavToggle } from './sidenav/sidenav.component';
import { LoadingSpinnerService } from './services/loading-spinner.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit{
  isLoading!: Observable<boolean>;

  constructor(private spinnerService: LoadingSpinnerService) { }

  isSideNavCollapsed = false;
  screenWidth = 0;

  ngOnInit():void {
    this.isLoading = this.spinnerService.loading$;
  }

  onToggleSideNav(data: SideNavToggle): void{
    this.screenWidth = data.screenWidth;
    this.isSideNavCollapsed = data.collapsed;
  }

}
