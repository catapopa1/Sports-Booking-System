import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { TextareaModule } from 'primeng/textarea';
import { TooltipModule } from 'primeng/tooltip';
import { environment } from '../../../environments/environment';
import { AuthService } from '../../core/services/auth.service';
import { UsersService } from '../../core/services/users.service';
import { UserProfileDto } from '../../core/models/user.models';
import { ChangePasswordDialogComponent } from './change-password-dialog/change-password-dialog';
import { SportsCardComponent } from '../../shared/components/sports-card/sports-card';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    FormsModule,
    ButtonModule,
    SkeletonModule,
    TextareaModule,
    TooltipModule,
    ChangePasswordDialogComponent,
    SportsCardComponent,
  ],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class ProfileComponent {
  private readonly usersService = inject(UsersService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(MessageService);

  readonly profile = signal<UserProfileDto | null>(null);
  readonly loading = signal<boolean>(true);
  readonly editingBio = signal<boolean>(false);
  readonly bioDraft = signal<string>('');
  readonly savingBio = signal<boolean>(false);
  readonly uploadingAvatar = signal<boolean>(false);
  readonly showPasswordDialog = signal<boolean>(false);

  readonly email = computed(() => this.auth.user()?.email ?? '');

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
    this.loadProfile();
  }

  async loadProfile(): Promise<void> {
    this.loading.set(true);
    try {
      const data = await this.usersService.getMyProfile();
      this.profile.set(data);
    } catch {
      // errorInterceptor surfaces the toast globally
    } finally {
      this.loading.set(false);
    }
  }

  startEditBio(): void {
    this.bioDraft.set(this.profile()?.bio ?? '');
    this.editingBio.set(true);
  }

  cancelEditBio(): void {
    this.editingBio.set(false);
    this.bioDraft.set('');
  }

  async saveBio(): Promise<void> {
    this.savingBio.set(true);
    try {
      const draft = this.bioDraft().trim();
      await this.usersService.updateBio(draft.length === 0 ? null : draft);
      this.toast.add({ severity: 'success', summary: 'Saved', detail: 'Your bio has been updated.' });
      this.editingBio.set(false);
      await this.loadProfile();
    } finally {
      this.savingBio.set(false);
    }
  }

  async onAvatarSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      this.toast.add({ severity: 'warn', summary: 'Invalid file', detail: 'Please choose an image file.' });
      input.value = '';
      return;
    }

    this.uploadingAvatar.set(true);
    try {
      await this.usersService.uploadAvatar(file);
      this.toast.add({ severity: 'success', summary: 'Avatar updated' });
      await this.loadProfile();
    } finally {
      this.uploadingAvatar.set(false);
      input.value = '';
    }
  }

  roleBadgeData(role: string | undefined): string {
    return role ?? 'Player';
  }
}
