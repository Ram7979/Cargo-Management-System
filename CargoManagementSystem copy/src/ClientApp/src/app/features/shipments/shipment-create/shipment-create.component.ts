import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { ShipmentService } from '../../../core/services/shipment.service';
import { CustomerService } from '../../../core/services/customer.service';
import { NotificationService } from '../../../core/services/notification.service';
import { AuthService } from '../../../core/services/auth.service';
import { ApiResponse } from '../../../core/models/api-response.model';

@Component({
  selector: 'app-shipment-create',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    MatStepperModule, 
    MatFormFieldModule, 
    MatInputModule, 
    MatButtonModule, 
    MatSelectModule, 
    MatIconModule,
    MatDividerModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './shipment-create.component.html',
  styleUrls: ['./shipment-create.component.scss']
})
export class ShipmentCreateComponent implements OnInit {
  private fb = inject(FormBuilder);
  private shipmentService = inject(ShipmentService);
  private customerService = inject(CustomerService);
  private notification = inject(NotificationService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  senderFormGroup: FormGroup;
  recipientFormGroup: FormGroup;
  cargoFormGroup: FormGroup;
  isSubmitting = false;

  customers: any[] = [];
  // Backend CargoType enum values (must match exactly)
  cargoTypes = ['Standard', 'Fragile', 'Perishable', 'Hazardous'];
  serviceTypes = ['Standard', 'Express', 'Overnight', 'Economy'];
  paymentModes = ['Prepaid', 'Collect', 'ThirdParty'];

  constructor() {
    this.senderFormGroup = this.fb.group({
      customerId: ['', Validators.required],
      senderName: ['', Validators.required],
      senderContact: ['', Validators.required],
      senderZip: ['', Validators.required],
      originAddress: ['', Validators.required],
      senderCity: ['', Validators.required],
      senderCountry: ['', Validators.required]
    });

    this.recipientFormGroup = this.fb.group({
      recipientName: ['', Validators.required],
      recipientContact: ['', Validators.required],
      recipientZip: ['', Validators.required],
      destinationAddress: ['', Validators.required],
      recipientCity: ['', Validators.required],
      recipientCountry: ['', Validators.required]
    });

    this.cargoFormGroup = this.fb.group({
      cargoType: ['Standard', Validators.required],
      weightKg: [1, [Validators.required, Validators.min(0.1)]],
      volumeCbm: [0.1, [Validators.required, Validators.min(0.01)]],
      quantity: [1, [Validators.required, Validators.min(1)]],
      declaredValue: [0, [Validators.required, Validators.min(0)]],
      cargoDescription: ['', Validators.required],
      serviceType: ['Standard', Validators.required],
      paymentMode: ['Prepaid', Validators.required]
    });
  }

  ngOnInit() {
    this.loadCustomers();
    const preCustomerId = this.route.snapshot.queryParamMap.get('customerId');
    if (preCustomerId) {
      this.senderFormGroup.patchValue({ customerId: preCustomerId });
    }
  }

  loadCustomers() {
    this.customerService.getAll(1, 200).subscribe({
      next: (res) => {
        if (res.success && res.data?.items?.length) {
          this.customers = res.data.items;
        } else {
          this.notification.warning('No customers found. Please add a customer first.');
        }
      },
      error: () => this.notification.error('Failed to load customer list. Check your connection.')
    });
  }

  onSubmit() {
    if (this.senderFormGroup.invalid || this.recipientFormGroup.invalid || this.cargoFormGroup.invalid) {
      // Mark all fields as touched to show validation errors
      this.senderFormGroup.markAllAsTouched();
      this.recipientFormGroup.markAllAsTouched();
      this.cargoFormGroup.markAllAsTouched();
      this.notification.error('Please fill in all required fields');
      return;
    }

    this.isSubmitting = true;

    // Build the payload matching the backend CreateShipmentRequest exactly
    const senderVals = this.senderFormGroup.value;
    const recipientVals = this.recipientFormGroup.value;
    const cargoVals = this.cargoFormGroup.value;

    const shipmentData = {
      customerId: senderVals.customerId,
      // Sender
      senderName: senderVals.senderName,
      originAddress: senderVals.originAddress,
      senderCity: senderVals.senderCity,
      senderZip: senderVals.senderZip,
      senderCountry: senderVals.senderCountry,
      senderContact: senderVals.senderContact,
      // Recipient
      recipientName: recipientVals.recipientName,
      destinationAddress: recipientVals.destinationAddress,
      recipientCity: recipientVals.recipientCity,
      recipientZip: recipientVals.recipientZip,
      recipientCountry: recipientVals.recipientCountry,
      recipientContact: recipientVals.recipientContact,
      // Cargo
      weightKg: Number(cargoVals.weightKg),
      volumeCbm: Number(cargoVals.volumeCbm),
      quantity: Number(cargoVals.quantity),
      cargoType: cargoVals.cargoType,
      declaredValue: Number(cargoVals.declaredValue),
      cargoDescription: cargoVals.cargoDescription,
      serviceType: cargoVals.serviceType,
      paymentMode: cargoVals.paymentMode
    };

    this.shipmentService.create(shipmentData).subscribe({
      next: (res: ApiResponse<any>) => {
        if (res.success) {
          this.notification.success('Shipment created successfully!');
          
          // Generate real notification
          this.notification.createNotification({
            recipientId: shipmentData.customerId,
            channel: 'System',
            subject: 'Shipment Booked',
            body: `Your shipment has been successfully booked.`,
            eventType: 'ShipmentBooked'
          }).subscribe();

          const createdId = res.data?.id || res.data?.shipmentId;
          this.router.navigate(['/shipments', createdId]);
        } else {
          const errors = res.errors?.join(', ') || res.message || 'Failed to create shipment';
          this.notification.error(errors);
        }
        this.isSubmitting = false;
      },
      error: (err: any) => {
        this.isSubmitting = false;
        const msg = err.error?.errors?.join(', ') || err.error?.message || err.message || 'Failed to create shipment';
        this.notification.error(msg);
      }
    });
  }
}
