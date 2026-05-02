import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { BaseCrudService } from './base-crud.service';

@Injectable({ providedIn: 'root' })
export class FleetService extends BaseCrudService<any> {
  constructor() {
    super(inject(HttpClient), `${environment.apiUrl}/vehicles`);
  }

  getLiveLocations() {
    return this.http.get<any>(`${this.apiUrl}/live-locations`);
  }

  assignShipment(assignment: any) {
    return this.http.post<any>(`${environment.apiUrl}/assignments`, assignment);
  }
}
