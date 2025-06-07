import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { LoadingSpinnerService } from '../common/services/loading-spinner.service';
import { LoadingSpinnerComponent } from '../common/components/loading-spinner/loading-spinner.component';
import { SidenavComponent } from './components/sidenav/sidenav.component';
import { CommonModule } from '@angular/common';
import { IonContent, IonRouterOutlet } from '@ionic/angular/standalone';
import { PlayerComponent } from './components/player/player.component';
import { PlayerService } from './services/player.service';
import { CatalogService } from './services/catalog.service';
import { LibraryService } from './services/library.service';

@Component({
  selector: 'app-home',
  templateUrl: 'home.component.html',
  styleUrls: ['home.component.scss'],
  providers: [CatalogService, PlayerService, LibraryService],
  imports: [
    PlayerComponent,
    LoadingSpinnerComponent,
    SidenavComponent,
    CommonModule,
    IonRouterOutlet,
    IonContent
  ]
})
export class HomeComponent implements OnInit{
  public isLoading$!: Observable<boolean>;
  public playerActive$!: Observable<boolean>;

  constructor(
    private spinnerService: LoadingSpinnerService,
    private playerService: PlayerService,
  ) { }

  ngOnInit():void {
    this.playerActive$ = this.playerService.playerActive$;
    this.isLoading$ = this.spinnerService.loading$;
  }

}
