import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AvatarModule } from 'primeng/avatar';
import { TooltipModule } from 'primeng/tooltip';
import { AuthService } from '../../core/services/auth.service';
import { ThemeService } from '../../core/services/theme.service';

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
  readonly collapsed = signal(false);

  readonly userInitial = computed(() => (this.auth.user()?.email ?? '?')[0].toUpperCase());

  private readonly allNavItems: NavItem[] = [
    { label: 'Dashboard',     icon: 'pi pi-home',     route: '/dashboard' },
    { label: 'Parks',         icon: 'pi pi-map',      route: '/parks' },
    { label: 'Bookings',      icon: 'pi pi-calendar', route: '/bookings' },
    { label: 'Friends',       icon: 'pi pi-users',    route: '/friends' },
    { label: 'Notifications', icon: 'pi pi-bell',     route: '/notifications' },
    { label: 'Profile',       icon: 'pi pi-user',     route: '/profile' },
    { label: 'Admin',         icon: 'pi pi-shield',   route: '/admin', roles: ['Admin'] },
  ];

  readonly navItems = computed(() => {
    const role = this.auth.role();
    return this.allNavItems.filter(item => !item.roles || item.roles.includes(role ?? ''));
  });

  toggle(): void {
    this.collapsed.update(v => !v);
  }
}
