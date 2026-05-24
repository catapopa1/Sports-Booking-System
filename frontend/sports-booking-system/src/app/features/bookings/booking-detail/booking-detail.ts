import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { SkeletonModule } from 'primeng/skeleton';
import { BookingsService } from '../../../core/services/bookings.service';
import { BookingDto, BookingStatus } from '../../../core/models/booking.models';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip';

const CANCELLABLE_STATUSES: BookingStatus[] = [
  'Requested',
  'PendingPlayerConfirmations',
  'PendingManagerApproval',
];

@Component({
  selector: 'app-booking-detail',
  standalone: true,
  imports: [
    DatePipe,
    CurrencyPipe,
    RouterLink,
    ButtonModule,
    ConfirmDialogModule,
    SkeletonModule,
    StatusChipComponent,
  ],
  providers: [ConfirmationService],
  templateUrl: './booking-detail.html',
  styleUrl: './booking-detail.scss',
})
export class BookingDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly bookings = inject(BookingsService);
  private readonly toast = inject(MessageService);
  private readonly confirm = inject(ConfirmationService);

  readonly booking = signal<BookingDto | null>(null);
  readonly loading = signal(true);
  readonly loadError = signal(false);
  readonly cancelling = signal(false);

  readonly canCancel = computed(() => {
    const b = this.booking();
    if (!b || this.cancelling()) return false;
    return CANCELLABLE_STATUSES.includes(b.status as BookingStatus);
  });

  readonly acceptedInvites = computed(() =>
    this.booking()?.invites.filter(i => i.status === 'Accepted').length ?? 0
  );

  constructor() {
    this.load();
  }

  async load(): Promise<void> {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = Number(idParam);
    if (!idParam || Number.isNaN(id)) {
      this.loadError.set(true);
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    this.loadError.set(false);
    try {
      const b = await this.bookings.getById(id);
      this.booking.set(b);
    } catch {
      this.loadError.set(true);
    } finally {
      this.loading.set(false);
    }
  }

  promptCancel(): void {
    const b = this.booking();
    if (!b) return;

    this.confirm.confirm({
      header: 'Cancel booking',
      message: `Cancel your booking at ${b.parkName} – ${b.fieldName}? Invited friends will be notified.`,
      acceptLabel: 'Cancel booking',
      rejectLabel: 'Keep it',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.doCancel(b.id),
    });
  }

  private async doCancel(id: number): Promise<void> {
    this.cancelling.set(true);
    try {
      await this.bookings.cancel(id);
      this.toast.add({ severity: 'success', summary: 'Booking cancelled' });
      this.router.navigate(['/bookings']);
    } catch {
      this.cancelling.set(false);
    }
  }
}
