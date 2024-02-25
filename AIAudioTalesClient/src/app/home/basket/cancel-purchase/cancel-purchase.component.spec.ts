import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CancelPurchaseComponent } from './cancel-purchase.component';

describe('CancelPurchaseComponent', () => {
  let component: CancelPurchaseComponent;
  let fixture: ComponentFixture<CancelPurchaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CancelPurchaseComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CancelPurchaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
