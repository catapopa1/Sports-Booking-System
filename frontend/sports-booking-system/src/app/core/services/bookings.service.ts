import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  BookingDto,
  BookingSummaryDto,
  CreateBookingRequest,
  InviteNotificationDto,
  InviteStatus,
} from '../models/booking.models';
import { PagedResult } from '../models/pagination.models';

@Injectable({ providedIn: 'root' })
export class BookingsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/Bookings`;

  create(request: CreateBookingRequest): Promise<{ id: number }> {
    return firstValueFrom(this.http.post<{ id: number }>(this.base, request));
  }

  getById(id: number): Promise<BookingDto> {
    return firstValueFrom(this.http.get<BookingDto>(`${this.base}/${id}`));
  }

  getMine(page = 1, pageSize = 20): Promise<PagedResult<BookingSummaryDto>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return firstValueFrom(
      this.http.get<PagedResult<BookingSummaryDto>>(`${this.base}/mine`, { params })
    );
  }

  getMyInvites(): Promise<InviteNotificationDto[]> {
    return firstValueFrom(this.http.get<InviteNotificationDto[]>(`${this.base}/invites`));
  }

  getPendingApprovals(): Promise<BookingSummaryDto[]> {
    return firstValueFrom(
      this.http.get<BookingSummaryDto[]>(`${this.base}/pending-approvals`)
    );
  }

  respondToInvite(bookingId: number, status: InviteStatus): Promise<void> {
    return firstValueFrom(
      this.http.put<void>(
        `${this.base}/${bookingId}/respond`,
        JSON.stringify(status),
        { headers: { 'Content-Type': 'application/json' } }
      )
    );
  }

  approve(bookingId: number): Promise<void> {
    return firstValueFrom(this.http.put<void>(`${this.base}/${bookingId}/approve`, null));
  }

  cancel(bookingId: number): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.base}/${bookingId}`));
  }
}
