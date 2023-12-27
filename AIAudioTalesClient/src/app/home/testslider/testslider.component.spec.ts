import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestsliderComponent } from './testslider.component';

describe('TestsliderComponent', () => {
  let component: TestsliderComponent;
  let fixture: ComponentFixture<TestsliderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestsliderComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestsliderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
