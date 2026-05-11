import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReceiveCargoComponent } from './receive-cargo.component';

describe('ReceiveCargoComponent', () => {
  let component: ReceiveCargoComponent;
  let fixture: ComponentFixture<ReceiveCargoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReceiveCargoComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ReceiveCargoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
