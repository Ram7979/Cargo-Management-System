import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { BillingService } from '../../../core/services/billing.service';
import { ShipmentService } from '../../../core/services/shipment.service';
import { CustomerService } from '../../../core/services/customer.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ApiResponse } from '../../../core/models/api-response.model';
import { PagedResult } from '../../../core/services/base-api.service';
import { Shipment } from '../../../core/services/shipment.service';

@Component({
  selector: 'app-invoice-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, MatFormFieldModule, MatInputModule,
            MatButtonModule, MatSelectModule, MatDatepickerModule, MatNativeDateModule,
            MatCardModule, MatIconModule, MatDividerModule],
  templateUrl: './invoice-create.component.html'
})
export class InvoiceCreateComponent implements OnInit {
  private fb = inject(FormBuilder);
  private billingService = inject(BillingService);
  private shipmentService = inject(ShipmentService);
  private customerService = inject(CustomerService);
  private notification = inject(NotificationService);
  private router = inject(Router);

  form = this.fb.group({
    shipmentId:        ['', Validators.required],
    customerId:        ['', Validators.required],
    baseFreightCharge: [0, [Validators.required, Validators.min(0)]],
    fuelSurcharge:     [0, [Validators.required, Validators.min(0)]],
    handlingFee:       [0, [Validators.required, Validators.min(0)]],
    insuranceAmount:   [0, [Validators.required, Validators.min(0)]],
    taxRate:           [0.18, [Validators.required, Validators.min(0)]],
    dueDate:           [null as Date | null],
    notes:             ['']
  });

  shipments: any[] = [];
  customers: any[] = [];
  isSubmitting = false;

  get computedTotal(): number {
    const v = this.form.value;
    const sub = Number(v.baseFreightCharge || 0) + Number(v.fuelSurcharge || 0)
              + Number(v.handlingFee || 0) + Number(v.insuranceAmount || 0);
    return sub + sub * Number(v.taxRate || 0);
  }

  ngOnInit() {
    this.customerService.getAll(1, 200).subscribe((res: ApiResponse<PagedResult<any>>) => {
      if (res.success && res.data) this.customers = res.data.items;
    });
    this.shipmentService.getAll(1, 200).subscribe((res: ApiResponse<PagedResult<Shipment>>) => {
      if (res.success && res.data) this.shipments = res.data.items;
    });
  }

  onSubmit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.isSubmitting = true;
    const v = this.form.value;
    const payload = {
      shipmentId:        v.shipmentId,
      customerId:        v.customerId,
      baseFreightCharge: Number(v.baseFreightCharge),
      fuelSurcharge:     Number(v.fuelSurcharge),
      handlingFee:       Number(v.handlingFee),
      insuranceAmount:   Number(v.insuranceAmount),
      taxRate:           Number(v.taxRate),
      dueDate:           v.dueDate ? new Date(v.dueDate!).toISOString() : null,
      notes:             v.notes || ''
    };
    this.billingService.create(payload).subscribe({
      next: (res: any) => {
        if (res.success) {
          this.notification.success('Invoice created successfully! PDF will be generated automatically.');
          this.router.navigate(['/billing', res.data?.id]);
        } else {
          this.notification.error(res.errors?.join(', ') || res.message || 'Failed to create invoice');
        }
        this.isSubmitting = false;
      },
      error: (err: any) => {
        this.notification.error(err.error?.errors?.join(', ') || err.error?.message || 'Failed to create invoice');
        this.isSubmitting = false;
      }
    });
  }
}
