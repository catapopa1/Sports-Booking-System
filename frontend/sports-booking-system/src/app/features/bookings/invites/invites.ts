import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { BookingsService } from '../../../core/services/bookings.service';
import { InviteNotificationDto, InviteStatus } from '../../../core/models/booking.models';

@Component({
  selector: 'app-invites',
  standalone: true,
  imports: [DatePipe, RouterLink, ButtonModule, SkeletonModule],
  templateUrl: './invites.html',
  styleUrl: './invites.scss',
})
export class InvitesComponent {
  private readonly bookings = inject(BookingsService);
  private readonly toast = inject(MessageService);

  readonly invites = signal<InviteNotificationDto[]>([]);
  readonly loading = signal(true);
  readonly respondingTo = signal<Set<number>>(new Set());

  readonly isEmpty = computed(() => !this.loading() && this.invites().length === 0);

  constructor() {
    this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    try {
      const list = await this.bookings.getMyInvites();
      this.invites.set(list);
      this.bookings.refreshInvitesCount();
    } finally {
      this.loading.set(false);
    }
  }

  isResponding(bookingId: number): boolean {
    return this.respondingTo().has(bookingId);
  }

  async respond(bookingId: number, status: InviteStatus): Promise<void> {
    if (this.isResponding(bookingId)) return;

    const previous = this.invites();
    this.invites.set(previous.filter(i => i.bookingId !== bookingId));
    this.respondingTo.update(set => new Set(set).add(bookingId));

    try {
      await this.bookings.respondToInvite(bookingId, status);
      this.bookings.refreshInvitesCount();
      this.toast.add({
        severity: status === 'Accepted' ? 'success' : 'info',
        summary: status === 'Accepted' ? 'Invite accepted' : 'Invite declined',
      });
    } catch {
      this.invites.set(previous);
    } finally {
      this.respondingTo.update(set => {
        const next = new Set(set);
        next.delete(bookingId);
        return next;
      });
    }
  }
}
