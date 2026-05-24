import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SkeletonModule } from 'primeng/skeleton';
import { BookingsService } from '../../../core/services/bookings.service';
import { BookingSummaryDto } from '../../../core/models/booking.models';
import { PagedResult } from '../../../core/models/pagination.models';
import { StatusChipComponent } from '../../../shared/components/status-chip/status-chip';

@Component({
  selector: 'app-my-bookings',
  standalone: true,
  imports: [
    DatePipe,
    CurrencyPipe,
    RouterLink,
    ButtonModule,
    PaginatorModule,
    SkeletonModule,
    StatusChipComponent,
  ],
  templateUrl: './my-bookings.html',
  styleUrl: './my-bookings.scss',
})
export class MyBookingsComponent {
  private readonly bookings = inject(BookingsService);

  readonly page = signal(1);
  readonly pageSize = signal(10);
  readonly loading = signal(true);
  readonly result = signal<PagedResult<BookingSummaryDto> | null>(null);

  readonly first = computed(() => (this.page() - 1) * this.pageSize());
  readonly items = computed(() => this.result()?.items ?? []);
  readonly totalCount = computed(() => this.result()?.totalCount ?? 0);
  readonly isEmpty = computed(() => !this.loading() && this.items().length === 0);

  constructor() {
    this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    try {
      const res = await this.bookings.getMine(this.page(), this.pageSize());
      this.result.set(res);
    } finally {
      this.loading.set(false);
    }
  }

  onPageChange(event: PaginatorState): void {
    const newPage = ((event.first ?? 0) / (event.rows ?? this.pageSize())) + 1;
    const newSize = event.rows ?? this.pageSize();
    if (newPage === this.page() && newSize === this.pageSize()) return;
    this.page.set(newPage);
    this.pageSize.set(newSize);
    this.load();
  }
}
