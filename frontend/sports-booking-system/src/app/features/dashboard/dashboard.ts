import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { AuthService } from '../../core/services/auth.service';
import { BookingsService } from '../../core/services/bookings.service';
import { FriendshipsService } from '../../core/services/friendships.service';
import { NotificationsService } from '../../core/services/notifications.service';
import { UsersService } from '../../core/services/users.service';
import { BookingStatus, BookingSummaryDto, InviteNotificationDto } from '../../core/models/booking.models';
import { NotificationDto } from '../../core/models/notification.models';
import { UserProfileDto } from '../../core/models/user.models';
import { StatusChipComponent } from '../../shared/components/status-chip/status-chip';

const ACTIVE_STATUSES: BookingStatus[] = [
  'Requested',
  'PendingPlayerConfirmations',
  'Confirmed',
];

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    DatePipe,
    CurrencyPipe,
    RouterLink,
    ButtonModule,
    SkeletonModule,
    StatusChipComponent,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class DashboardComponent {
  readonly auth = inject(AuthService);
  readonly bookings = inject(BookingsService);
  readonly notifications = inject(NotificationsService);
  private readonly friendships = inject(FriendshipsService);
  private readonly users = inject(UsersService);

  readonly loading = signal(true);
  readonly profile = signal<UserProfileDto | null>(null);
  readonly nextGame = signal<BookingSummaryDto | null>(null);
  readonly recentInvites = signal<InviteNotificationDto[]>([]);
  readonly recentActivity = signal<NotificationDto[]>([]);
  readonly friendsCount = signal(0);

  readonly firstName = computed(() => {
    const full = this.profile()?.fullName ?? this.auth.user()?.email ?? '';
    return full.split(/\s+/)[0] ?? '';
  });

  constructor() {
    this.load();
  }

  async load(): Promise<void> {
    this.loading.set(true);
    try {
      const [profile, bookings, invites, notifs, friends] = await Promise.all([
        this.users.getMyProfile().catch(() => null),
        this.bookings.getMine(1, 20).catch(() => null),
        this.bookings.getMyInvites().catch(() => []),
        this.notifications.getPage(1, 5).catch(() => null),
        this.friendships.getMyFriends().catch(() => []),
      ]);

      this.profile.set(profile);
      this.friendsCount.set(friends.length);
      this.recentInvites.set(invites.slice(0, 3));
      this.recentActivity.set(notifs?.items ?? []);

      const now = Date.now();
      const upcoming = (bookings?.items ?? [])
        .filter(b =>
          ACTIVE_STATUSES.includes(b.status as BookingStatus)
          && new Date(b.startTime).getTime() > now)
        .sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime());

      this.nextGame.set(upcoming[0] ?? null);
    } finally {
      this.loading.set(false);
    }
  }
}
