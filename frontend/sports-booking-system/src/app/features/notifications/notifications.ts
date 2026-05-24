import { Component, computed, effect, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SkeletonModule } from 'primeng/skeleton';
import { NotificationsService } from '../../core/services/notifications.service';
import { NotificationDto } from '../../core/models/notification.models';
import { PagedResult } from '../../core/models/pagination.models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [DatePipe, ButtonModule, PaginatorModule, SkeletonModule],
  templateUrl: './notifications.html',
  styleUrl: './notifications.scss',
})
export class NotificationsComponent {
  private readonly service = inject(NotificationsService);
  private readonly toast = inject(MessageService);

  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly loading = signal(true);
  readonly result = signal<PagedResult<NotificationDto> | null>(null);
  readonly markingAll = signal(false);

  readonly first = computed(() => (this.page() - 1) * this.pageSize());
  readonly items = computed(() => this.result()?.items ?? []);
  readonly totalCount = computed(() => this.result()?.totalCount ?? 0);
  readonly isEmpty = computed(() => !this.loading() && this.items().length === 0);
  readonly hasUnreadOnPage = computed(() => this.items().some(n => !n.isRead));

  constructor() {
    this.load();

    // When a live notification arrives, prepend it to the page-1 list so the user sees it without refreshing.
    effect(() => {
      const incoming = this.service.lastReceived();
      if (!incoming) return;
      if (this.page() !== 1) return;

      const current = this.result();
      if (!current) return;

      const deduped = current.items.filter(n => n.id !== incoming.id);
      const items = [incoming, ...deduped].slice(0, this.pageSize());
      this.result.set({ ...current, items, totalCount: current.totalCount + 1 });
    });
  }

  async load(): Promise<void> {
    this.loading.set(true);
    try {
      const res = await this.service.getPage(this.page(), this.pageSize());
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

  async markRead(n: NotificationDto): Promise<void> {
    if (n.isRead) return;

    const current = this.result();
    if (!current) return;

    // Optimistic: flip the flag in place, decrement the badge.
    const items = current.items.map(x => x.id === n.id ? { ...x, isRead: true } : x);
    this.result.set({ ...current, items });
    this.service.decrementUnread();

    try {
      await this.service.markRead(n.id);
    } catch {
      // Roll back on failure.
      this.result.set(current);
      this.service.refreshUnreadCount();
    }
  }

  async markAllRead(): Promise<void> {
    if (this.markingAll() || !this.hasUnreadOnPage()) return;

    const current = this.result();
    this.markingAll.set(true);
    try {
      await this.service.markAllRead();
      if (current) {
        const items = current.items.map(x => ({ ...x, isRead: true }));
        this.result.set({ ...current, items });
      }
      this.service.clearUnread();
      this.toast.add({ severity: 'success', summary: 'All notifications marked read' });
    } finally {
      this.markingAll.set(false);
    }
  }
}
