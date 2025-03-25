import { Component, OnInit } from "@angular/core";
import { LoadingSpinnerComponent } from "../common/components/loading-spinner/loading-spinner.component";
import { CommonModule } from "@angular/common";
import { IonContent, IonRouterOutlet } from "@ionic/angular/standalone";
import { Observable } from "rxjs";
import { AuthService } from "../auth/services/auth.service";
import { LoadingSpinnerService } from "../common/services/loading-spinner.service";
import { CreatorSidenavComponent } from "./components/creator-sidenav/creator-sidenav.component";
import { CreatorService } from "./services/creator.service";

@Component({
  selector: 'app-creator-home',
  templateUrl: 'creator-home.component.html',
  styleUrls: ['creator-home.component.scss'],
  providers: [CreatorService],
  imports: [
    LoadingSpinnerComponent,
    CreatorSidenavComponent,
    CommonModule,
    IonRouterOutlet,
    IonContent
  ]
})
export class CreatorHomeComponent implements OnInit{
  public isLoading$!: Observable<boolean>;

  constructor(
    private spinnerService: LoadingSpinnerService
  ) { }

  ngOnInit():void {
    this.isLoading$ = this.spinnerService.loading$;
  }

}
