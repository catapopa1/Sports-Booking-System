import { Component, computed, inject, input, output, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { SportLevel, SportType, UserSportSummaryDto } from '../../../core/models/user.models';
import { UsersService } from '../../../core/services/users.service';

const ALL_SPORTS: SportType[] = ['Football', 'Tennis', 'Basketball'];

const LEVEL_OPTIONS: { label: string; value: SportLevel | null }[] = [
  { label: 'Not set', value: null },
  { label: 'Beginner', value: 'Beginner' },
  { label: 'Intermediate', value: 'Intermediate' },
  { label: 'Advanced', value: 'Advanced' },
  { label: 'Professional', value: 'Professional' },
];

@Component({
  selector: 'app-sports-card',
  standalone: true,
  imports: [FormsModule, ButtonModule, DialogModule, InputTextModule, SelectModule],
  templateUrl: './sports-card.html',
  styleUrl: './sports-card.scss',
})
export class SportsCardComponent {
  private readonly usersService = inject(UsersService);
  private readonly toast = inject(MessageService);

  readonly sports = input.required<UserSportSummaryDto[]>();
  readonly isReadonly = input<boolean>(false);

  readonly changed = output<void>();

  readonly levelOptions = LEVEL_OPTIONS;

  readonly editingSport = signal<SportType | null>(null);
  readonly draftLevel = signal<SportLevel | null>(null);
  readonly draftFavoriteAthlete = signal<string>('');
  readonly saving = signal<boolean>(false);

  readonly visibleSports = computed(() => {
    const map = new Map(this.sports().map(s => [s.sport, s]));
    const merged: UserSportSummaryDto[] = ALL_SPORTS.map(sport =>
      map.get(sport) ?? {
        sport, level: null, favoriteAthlete: null, matchesPlayed: 0,
      });
    return this.isReadonly()
      ? merged.filter(s => s.level !== null || !!s.favoriteAthlete || s.matchesPlayed > 0)
      : merged;
  });

  startEdit(s: UserSportSummaryDto): void {
    this.draftLevel.set(s.level);
    this.draftFavoriteAthlete.set(s.favoriteAthlete ?? '');
    this.editingSport.set(s.sport);
  }

  cancel(): void {
    if (this.saving()) return;
    this.editingSport.set(null);
  }

  onDialogVisibleChange(visible: boolean): void {
    if (!visible) this.cancel();
  }

  async save(): Promise<void> {
    const sport = this.editingSport();
    if (!sport) return;
    this.saving.set(true);
    try {
      const trimmed = this.draftFavoriteAthlete().trim();
      await this.usersService.upsertSportProfile(sport, {
        level: this.draftLevel(),
        favoriteAthlete: trimmed.length === 0 ? null : trimmed,
      });
      this.toast.add({ severity: 'success', summary: 'Saved' });
      this.editingSport.set(null);
      this.changed.emit();
    } finally {
      this.saving.set(false);
    }
  }
}
