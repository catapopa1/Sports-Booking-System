import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SelectModule } from 'primeng/select';
import { SkeletonModule } from 'primeng/skeleton';
import { AdminService } from '../../../core/services/admin.service';
import { AuthService } from '../../../core/services/auth.service';
import { AdminUserDto, UserRole } from '../../../core/models/user.models';
import { PagedResult } from '../../../core/models/pagination.models';

interface RoleOption { label: string; value: UserRole; }

@Component({
  selector: 'app-admin-console',
  standalone: true,
  imports: [
    FormsModule,
    ButtonModule,
    ConfirmDialogModule,
    IconFieldModule,
    InputIconModule,
    InputTextModule,
    PaginatorModule,
    SelectModule,
    SkeletonModule,
  ],
  providers: [ConfirmationService],
  templateUrl: './admin-console.html',
  styleUrl: './admin-console.scss',
})
export class AdminConsoleComponent {
  private readonly admin = inject(AdminService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(MessageService);
  private readonly confirm = inject(ConfirmationService);

  private searchTimeout?: number;

  readonly query = signal('');
  readonly page = signal(1);
  readonly pageSize = signal(20);
  readonly loading = signal(true);
  readonly updatingIds = signal<Set<number>>(new Set());
  readonly result = signal<PagedResult<AdminUserDto> | null>(null);

  readonly roles: RoleOption[] = [
    { label: 'Admin',        value: 'Admin' },
    { label: 'Park manager', value: 'ParkManager' },
    { label: 'Player',       value: 'Player' },
  ];

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
      const res = await this.admin.getUsers(this.query(), this.page(), this.pageSize());
      this.result.set(res);
    } finally {
      this.loading.set(false);
    }
  }

  onSearchInput(value: string): void {
    this.query.set(value);
    if (this.searchTimeout) clearTimeout(this.searchTimeout);
    this.searchTimeout = window.setTimeout(() => {
      this.page.set(1);
      this.load();
    }, 300);
  }

  onPageChange(event: PaginatorState): void {
    const newPage = ((event.first ?? 0) / (event.rows ?? this.pageSize())) + 1;
    const newSize = event.rows ?? this.pageSize();
    if (newPage === this.page() && newSize === this.pageSize()) return;
    this.page.set(newPage);
    this.pageSize.set(newSize);
    this.load();
  }

  isUpdating(userId: number): boolean {
    return this.updatingIds().has(userId);
  }

  isSelf(userId: number): boolean {
    return this.auth.userId() === userId;
  }

  initials(fullName: string): string {
    const parts = fullName.trim().split(/\s+/);
    const first = parts[0]?.[0] ?? '';
    const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
    return (first + last).toUpperCase() || '?';
  }

  onRoleChange(user: AdminUserDto, newRole: UserRole): void {
    if (newRole === user.role || this.isSelf(user.id)) return;

    this.confirm.confirm({
      header: 'Change user role',
      message: `Set ${user.fullName} as ${this.labelFor(newRole)}? They will gain or lose permissions immediately.`,
      acceptLabel: 'Change role',
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: newRole === 'Admin' ? 'p-button-warning' : '',
      accept: () => this.applyRoleChange(user, newRole),
      reject: () => this.revertRole(user),
    });
  }

  private async applyRoleChange(user: AdminUserDto, newRole: UserRole): Promise<void> {
    const previous = user.role;
    this.patchLocalRole(user.id, newRole);
    this.updatingIds.update(s => new Set(s).add(user.id));

    try {
      await this.admin.updateRole(user.id, newRole);
      this.toast.add({
        severity: 'success',
        summary: 'Role updated',
        detail: `${user.fullName} is now ${this.labelFor(newRole)}.`,
      });
    } catch {
      this.patchLocalRole(user.id, previous as UserRole);
    } finally {
      this.updatingIds.update(s => {
        const next = new Set(s);
        next.delete(user.id);
        return next;
      });
    }
  }

  /** Reset the dropdown's bound value when the user cancels the confirm. */
  private revertRole(user: AdminUserDto): void {
    const current = this.result();
    if (!current) return;
    // Force a new array reference so p-select re-reads the value.
    this.result.set({
      ...current,
      items: current.items.map(u => u.id === user.id ? { ...u } : u),
    });
  }

  private patchLocalRole(userId: number, role: UserRole): void {
    const current = this.result();
    if (!current) return;
    this.result.set({
      ...current,
      items: current.items.map(u => u.id === userId ? { ...u, role } : u),
    });
  }

  private labelFor(role: UserRole): string {
    return this.roles.find(r => r.value === role)?.label ?? role;
  }
}
