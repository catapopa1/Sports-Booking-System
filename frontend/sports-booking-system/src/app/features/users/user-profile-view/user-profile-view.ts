import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { environment } from '../../../../environments/environment';
import { UsersService } from '../../../core/services/users.service';
import { UserProfileDto } from '../../../core/models/user.models';
import { SportsCardComponent } from '../../../shared/components/sports-card/sports-card';

@Component({
  selector: 'app-user-profile-view',
  standalone: true,
  imports: [RouterLink, ButtonModule, SkeletonModule, SportsCardComponent],
  templateUrl: './user-profile-view.html',
  styleUrl: './user-profile-view.scss',
})
export class UserProfileViewComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly usersService = inject(UsersService);

  readonly profile = signal<UserProfileDto | null>(null);
  readonly loading = signal<boolean>(true);
  readonly notFound = signal<boolean>(false);

  readonly avatarUrl = computed(() => {
    const url = this.profile()?.profilePictureUrl;
    if (!url) return null;
    if (url.startsWith('http')) return url;
    return `${environment.apiBaseUrl}${url.startsWith('/') ? url : '/' + url}`;
  });

  readonly initials = computed(() => {
    const name = this.profile()?.fullName?.trim() ?? '';
    if (!name) return '?';
    const parts = name.split(/\s+/);
    const first = parts[0]?.[0] ?? '';
    const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
    return (first + last).toUpperCase();
  });

  constructor() {
    this.route.paramMap.subscribe(params => {
      const id = Number(params.get('id'));
      if (!Number.isFinite(id) || id <= 0) {
        this.router.navigate(['/dashboard']);
        return;
      }
      this.load(id);
    });
  }

  private async load(id: number): Promise<void> {
    this.loading.set(true);
    this.notFound.set(false);
    try {
      this.profile.set(await this.usersService.getById(id));
    } catch {
      this.notFound.set(true);
    } finally {
      this.loading.set(false);
    }
  }
}
