import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-status-chip',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="status-chip" [ngClass]="statusClass">
      {{ status }}
    </span>
  `,
  styles: [`
    .status-chip {
      padding: 4px 12px;
      border-radius: 16px;
      font-size: 0.75rem;
      font-weight: 600;
      text-transform: uppercase;
      display: inline-block;
      white-space: nowrap;
    }
    
    /* Shipment Statuses */
    .pending { background-color: #fef9c3; color: #854d0e; }
    .in-transit { background-color: #dbeafe; color: #1e40af; }
    .delivered { background-color: #dcfce7; color: #166534; }
    .cancelled { background-color: #fee2e2; color: #991b1b; }
    
    /* User Statuses */
    .active { background-color: #dcfce7; color: #166534; }
    .deactivated { background-color: #fee2e2; color: #991b1b; }
    
    /* Vehicle Statuses */
    .available { background-color: #dcfce7; color: #166534; }
    .on-trip { background-color: #dbeafe; color: #1e40af; }
    .maintenance { background-color: #fee2e2; color: #991b1b; }
    
    /* Billing Statuses */
    .paid { background-color: #dcfce7; color: #166534; }
    .overdue { background-color: #fee2e2; color: #991b1b; }
    .void { background-color: #f3f4f6; color: #4b5563; }
  `]
})
export class StatusChipComponent {
  @Input() status: string = '';
  
  get statusClass(): string {
    return this.status.toLowerCase().replace(/\s+/g, '-');
  }
}
