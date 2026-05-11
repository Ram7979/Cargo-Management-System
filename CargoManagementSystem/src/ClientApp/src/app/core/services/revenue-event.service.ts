import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

/**
 * Singleton event bus that allows any component (e.g. payment dialog)
 * to notify the Admin Dashboard that revenue has changed and it should
 * refresh immediately instead of waiting for the 30-second poll.
 */
@Injectable({ providedIn: 'root' })
export class RevenueEventService {
  /** Emit this after a successful payment is recorded. */
  private _paymentSuccess$ = new Subject<void>();
  readonly paymentSuccess$ = this._paymentSuccess$.asObservable();

  notifyPaymentSuccess() {
    this._paymentSuccess$.next();
  }
}
