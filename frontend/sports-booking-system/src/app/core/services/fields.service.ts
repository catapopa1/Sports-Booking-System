import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FieldDto } from '../models/park.models';
import { BookingType } from '../models/booking.models';

export interface FieldOccupancySlot {
  startTime: string;
  bookingType: BookingType;
}

@Injectable({ providedIn: 'root' })
export class FieldsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/Fields`;

  getById(id: number): Promise<FieldDto> {
    return firstValueFrom(this.http.get<FieldDto>(`${this.base}/${id}`));
  }

  getAvailability(id: number, from: Date, to: Date): Promise<FieldOccupancySlot[]> {
    const params = new HttpParams()
      .set('from', from.toISOString())
      .set('to', to.toISOString());
    return firstValueFrom(
      this.http.get<FieldOccupancySlot[]>(`${this.base}/${id}/availability`, { params })
    );
  }
}
