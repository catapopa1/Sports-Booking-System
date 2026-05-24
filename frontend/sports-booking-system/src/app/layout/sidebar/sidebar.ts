import { Component, computed, effect, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AvatarModule } from 'primeng/avatar';
import { TooltipModule } from 'primeng/tooltip';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';
import { BookingsService } from '../../core/services/bookings.service';
import { NotificationsService } from '../../core/services/notifications.service';
import { ThemeService } from '../../core/services/theme.service';
import { UsersService } from '../../core/services/users.service';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles?: string[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, AvatarModule, TooltipModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class SidebarComponent {
  readonly auth = inject(AuthService);
  readonly theme = inject(ThemeService);
  readonly bookings = inject(BookingsService);
  readonly notifications = inject(NotificationsService);
  private readonly usersService = inject(UsersService);
  readonly collapsed = signal(false);

  /** Profile picture URL — loaded once after login from the user's profile. */
  readonly profilePictureUrl = signal<string | null>(null);
  /** Full name — loaded once after login from the user's profile. Falls back to email. */
  readonly fullName = signal<string | null>(null);

  readonly displayName = computed(() => this.fullName() ?? this.auth.user()?.email ?? '');

  readonly userInitial = computed(() => {
    const name = this.fullName()?.trim();
    if (name) {
      const parts = name.split(/\s+/);
      const first = parts[0]?.[0] ?? '';
      const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
      return (first + last).toUpperCase() || '?';
    }
    return (this.auth.user()?.email ?? '?')[0].toUpperCase();
  });

  readonly avatarUrl = computed(() => {
    const raw = this.profilePictureUrl();
    if (!raw) return null;
    if (raw.startsWith('http')) return raw;
    return `${environment.apiBaseUrl}${raw.startsWith('/') ? raw : '/' + raw}`;
  });

  private readonly allNavItems: NavItem[] = [
    { label: 'Dashboard',     icon: 'pi pi-home',     route: '/dashboard' },
    { label: 'Parks',         icon: 'pi pi-map',      route: '/parks' },
    { label: 'Bookings',      icon: 'pi pi-calendar', route: '/bookings' },
    { label: 'Invites',       icon: 'pi pi-envelope', route: '/invites' },
    { label: 'Friends',       icon: 'pi pi-users',    route: '/friends' },
    { label: 'Notifications', icon: 'pi pi-bell',     route: '/notifications' },
    { label: 'Admin',         icon: 'pi pi-shield',   route: '/admin', roles: ['Admin'] },
  ];

  readonly navItems = computed(() => {
    const role = this.auth.role();
    return this.allNavItems.filter(item => !item.roles || item.roles.includes(role ?? ''));
  });

  constructor() {
    // Load profile picture + counters + open the SignalR connection whenever the user is logged in.
    effect(() => {
      const user = this.auth.user();
      if (user) {
        this.usersService.getMyProfile()
          .then(p => {
            this.profilePictureUrl.set(p.profilePictureUrl);
            this.fullName.set(p.fullName);
          })
          .catch(() => {
            this.profilePictureUrl.set(null);
            this.fullName.set(null);
          });
        this.bookings.refreshInvitesCount();
        this.notifications.refreshUnreadCount();
        this.notifications.connect(user.token);
      } else {
        this.profilePictureUrl.set(null);
        this.fullName.set(null);
        this.bookings.clearInvitesCount();
        this.notifications.clearUnread();
        this.notifications.disconnect();
      }
    });

    // When a notification arrives via SignalR and it relates to invites, bump that badge too.
    effect(() => {
      const last = this.notifications.lastReceived();
      if (!last) return;
      const t = last.title.toLowerCase();
      if (t.includes('invite') || t.includes('confirmed') || t.includes('cancelled')) {
        this.bookings.refreshInvitesCount();
      }
    });
  }

  toggle(): void {
    this.collapsed.update(v => !v);
  }
}
