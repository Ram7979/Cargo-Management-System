import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ShipmentReportComponent } from './shipment-report.component';

describe('ShipmentReportComponent', () => {
  let component: ShipmentReportComponent;
  let fixture: ComponentFixture<ShipmentReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ShipmentReportComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ShipmentReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
