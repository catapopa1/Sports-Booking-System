import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InputTextModule } from 'primeng/inputtext';
import { SkeletonModule } from 'primeng/skeleton';
import { TabsModule } from 'primeng/tabs';
import { BadgeModule } from 'primeng/badge';
import { environment } from '../../../environments/environment';
import { FriendshipsService } from '../../core/services/friendships.service';
import { UsersService } from '../../core/services/users.service';
import { FriendDto, FriendRequestDto } from '../../core/models/friendship.models';
import { UserSearchResultDto } from '../../core/models/user.models';

@Component({
  selector: 'app-friends',
  standalone: true,
  imports: [
    FormsModule,
    TabsModule,
    BadgeModule,
    ButtonModule,
    InputTextModule,
    SkeletonModule,
    ConfirmDialogModule,
  ],
  providers: [ConfirmationService],
  templateUrl: './friends.html',
  styleUrl: './friends.scss',
})
export class FriendsComponent {
  private readonly friendships = inject(FriendshipsService);
  private readonly users = inject(UsersService);
  private readonly toast = inject(MessageService);
  private readonly confirm = inject(ConfirmationService);

  readonly activeTab = signal<string>('friends');

  readonly friends = signal<FriendDto[]>([]);
  readonly pending = signal<FriendRequestDto[]>([]);
  readonly sent = signal<FriendRequestDto[]>([]);

  readonly loadingFriends = signal<boolean>(true);
  readonly loadingPending = signal<boolean>(true);
  readonly loadingSent = signal<boolean>(true);

  readonly searchInput = signal<string>('');
  readonly searchResults = signal<UserSearchResultDto[]>([]);
  readonly searching = signal<boolean>(false);
  readonly sentUserIds = signal<Set<number>>(new Set());
  readonly actingOn = signal<Set<number>>(new Set());

  readonly pendingCount = computed(() => this.pending().length);

  private searchTimeout?: number;

  constructor() {
    this.loadFriends();
    this.loadPending();
    this.loadSent();
  }

  avatarUrl(url: string | null): string | null {
    if (!url) return null;
    if (url.startsWith('http')) return url;
    return `${environment.apiBaseUrl}${url.startsWith('/') ? url : '/' + url}`;
  }

  relativeTime(iso: string): string {
    const date = new Date(iso);
    const diff = Date.now() - date.getTime();
    const minutes = Math.floor(diff / 60_000);
    if (minutes < 1) return 'just now';
    if (minutes < 60) return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    if (days < 7) return `${days}d ago`;
    return date.toLocaleDateString();
  }

  initials(fullName: string | null | undefined): string {
    const name = (fullName ?? '').trim();
    if (!name) return '?';
    const parts = name.split(/\s+/);
    const first = parts[0]?.[0] ?? '';
    const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
    return (first + last).toUpperCase();
  }

  async loadFriends(): Promise<void> {
    this.loadingFriends.set(true);
    try {
      this.friends.set(await this.friendships.getMyFriends());
    } finally {
      this.loadingFriends.set(false);
    }
  }

  async loadPending(): Promise<void> {
    this.loadingPending.set(true);
    try {
      this.pending.set(await this.friendships.getPending());
    } finally {
      this.loadingPending.set(false);
    }
  }

  async loadSent(): Promise<void> {
    this.loadingSent.set(true);
    try {
      this.sent.set(await this.friendships.getSent());
    } finally {
      this.loadingSent.set(false);
    }
  }

  onSearchInput(value: string): void {
    this.searchInput.set(value);
    if (this.searchTimeout) clearTimeout(this.searchTimeout);

    const query = value.trim();
    if (query.length < 2) {
      this.searchResults.set([]);
      this.searching.set(false);
      return;
    }

    this.searching.set(true);
    this.searchTimeout = window.setTimeout(() => this.runSearch(query), 300);
  }

  private async runSearch(query: string): Promise<void> {
    try {
      const result = await this.users.search(query, 1, 20);
      this.searchResults.set(result.items);
    } finally {
      this.searching.set(false);
    }
  }

  async sendRequest(userId: number): Promise<void> {
    this.trackAction(userId, true);
    try {
      await this.friendships.sendRequest(userId);
      this.sentUserIds.update(set => new Set(set).add(userId));
      this.toast.add({ severity: 'success', summary: 'Friend request sent' });
      this.loadSent();
    } finally {
      this.trackAction(userId, false);
    }
  }

  async accept(friendshipId: number): Promise<void> {
    this.trackAction(friendshipId, true);
    try {
      await this.friendships.respondToRequest(friendshipId, 'Accepted');
      this.toast.add({ severity: 'success', summary: 'Friend added' });
      await Promise.all([this.loadFriends(), this.loadPending()]);
    } finally {
      this.trackAction(friendshipId, false);
    }
  }

  async reject(friendshipId: number): Promise<void> {
    this.trackAction(friendshipId, true);
    try {
      await this.friendships.respondToRequest(friendshipId, 'Rejected');
      this.toast.add({ severity: 'success', summary: 'Request declined' });
      await this.loadPending();
    } finally {
      this.trackAction(friendshipId, false);
    }
  }

  async cancelSent(friendshipId: number): Promise<void> {
    this.trackAction(friendshipId, true);
    try {
      await this.friendships.removeFriend(friendshipId);
      this.toast.add({ severity: 'success', summary: 'Request cancelled' });
      await this.loadSent();
    } finally {
      this.trackAction(friendshipId, false);
    }
  }

  removeFriend(friendshipId: number, fullName: string): void {
    this.confirm.confirm({
      header: 'Remove friend',
      message: `Are you sure you want to remove ${fullName} from your friends?`,
      acceptLabel: 'Remove',
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: 'p-button-danger',
      accept: async () => {
        this.trackAction(friendshipId, true);
        try {
          await this.friendships.removeFriend(friendshipId);
          this.toast.add({ severity: 'success', summary: 'Friend removed' });
          await this.loadFriends();
        } finally {
          this.trackAction(friendshipId, false);
        }
      },
    });
  }

  private trackAction(id: number, active: boolean): void {
    this.actingOn.update(set => {
      const next = new Set(set);
      if (active) next.add(id);
      else next.delete(id);
      return next;
    });
  }
}
