import { Component, OnInit } from '@angular/core';
import { IonApp, IonRouterOutlet } from '@ionic/angular/standalone';
import { DialogHostComponent } from './common/components/dialog/base/dialog.host.component';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  imports: [IonApp, IonRouterOutlet, DialogHostComponent],
})
export class AppComponent implements OnInit {

  constructor() { }
  
  ngOnInit(): void { }
}
