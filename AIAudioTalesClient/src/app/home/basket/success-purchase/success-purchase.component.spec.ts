import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SuccessPurchaseComponent } from './success-purchase.component';

describe('SuccessPurchaseComponent', () => {
  let component: SuccessPurchaseComponent;
  let fixture: ComponentFixture<SuccessPurchaseComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SuccessPurchaseComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SuccessPurchaseComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
