import { DatePipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule, ReactiveFormsModule, NonNullableFormBuilder } from '@angular/forms';
import { takeUntilDestroyed, toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { DatePickerModule } from 'primeng/datepicker';
import { MessageModule } from 'primeng/message';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SkeletonModule } from 'primeng/skeleton';
import { environment } from '../../../../environments/environment';
import { BookingsService } from '../../../core/services/bookings.service';
import { FieldsService } from '../../../core/services/fields.service';
import { FriendshipsService } from '../../../core/services/friendships.service';
import { BookingType } from '../../../core/models/booking.models';
import { FieldDto, SportType } from '../../../core/models/park.models';
import { FriendDto } from '../../../core/models/friendship.models';

@Component({
  selector: 'app-create-booking',
  standalone: true,
  imports: [
    FormsModule,
    ReactiveFormsModule,
    RouterLink,
    DatePipe,
    ButtonModule,
    DatePickerModule,
    RadioButtonModule,
    MessageModule,
    SkeletonModule,
  ],
  templateUrl: './create-booking.html',
  styleUrl: './create-booking.scss',
})
export class CreateBookingComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly fields = inject(FieldsService);
  private readonly friendships = inject(FriendshipsService);
  private readonly bookings = inject(BookingsService);
  private readonly toast = inject(MessageService);
  private readonly fb = inject(NonNullableFormBuilder);

  readonly field = signal<FieldDto | null>(null);
  readonly friends = signal<FriendDto[]>([]);
  readonly loading = signal<boolean>(true);
  readonly loadError = signal<string | null>(null);
  readonly submitting = signal<boolean>(false);

  readonly minDate = new Date();
  readonly hours = Array.from({ length: 17 }, (_, i) => 6 + i); // 06:00 to 22:00

  readonly form = this.fb.group({
    date: this.fb.control<Date | null>(null),
    hour: this.fb.control<number | null>(null),
    bookingType: this.fb.control<BookingType>('Standard'),
    invitedPlayerIds: this.fb.control<number[]>([]),
  });

  readonly formValue = toSignal(this.form.valueChanges, { initialValue: this.form.getRawValue() });

  readonly sport = computed<SportType | undefined>(() => this.field()?.sportType as SportType | undefined);

  readonly isBasketball = computed(() => this.sport() === 'Basketball');

  readonly availableBookingTypes = computed<BookingType[]>(() =>
    this.isBasketball() ? ['FullCourt', 'HalfCourt'] : ['Standard']
  );

  readonly inviteRange = computed<{ min: number; max: number }>(() => {
    const s = this.sport();
    const bt = this.formValue().bookingType;
    if (s === 'Football') return { min: 5, max: 11 };
    if (s === 'Tennis') return { min: 1, max: 3 };
    if (s === 'Basketball' && bt === 'FullCourt') return { min: 3, max: 9 };
    if (s === 'Basketball' && bt === 'HalfCourt') return { min: 1, max: 5 };
    return { min: 0, max: 0 };
  });

  readonly maxInvites = computed(() => this.inviteRange().max);
  readonly minInvites = computed(() => this.inviteRange().min);

  readonly inviteCount = computed(() => this.formValue().invitedPlayerIds?.length ?? 0);

  readonly invitesValid = computed(() => {
    const { min, max } = this.inviteRange();
    const c = this.inviteCount();
    return max > 0 && c >= min && c <= max;
  });

  readonly canAddMoreInvites = computed(() => this.inviteCount() < this.maxInvites());

  readonly inviteHelperText = computed(() => {
    const s = this.sport();
    const bt = this.formValue().bookingType;
    const { min, max } = this.inviteRange();
    if (!s || max === 0) return '';
    const range = min === max ? `${min}` : `${min}–${max}`;
    if (s === 'Football') return `Football needs ${range} friends (total ${min + 1}–${max + 1} players).`;
    if (s === 'Tennis') return `Tennis needs ${range} friend${max === 1 ? '' : 's'} — 1v1 or doubles.`;
    if (s === 'Basketball' && bt === 'FullCourt') return `Full court needs ${range} friends (total ${min + 1}–${max + 1}).`;
    if (s === 'Basketball' && bt === 'HalfCourt') return `Half court needs ${range} friend${max === 1 ? '' : 's'} (total ${min + 1}–${max + 1}).`;
    return '';
  });

  readonly inviteStatusText = computed(() => {
    const { min, max } = this.inviteRange();
    const current = this.inviteCount();
    if (max === 0) return '';
    const range = min === max ? `${min}` : `${min}–${max}`;
    return `${current} selected — needs ${range}`;
  });

  readonly totalPrice = computed(() => {
    const price = this.field()?.baseHourlyPrice ?? 0;
    return this.formValue().bookingType === 'HalfCourt' ? price * 0.75 : price;
  });

  readonly hasDiscount = computed(() => this.formValue().bookingType === 'HalfCourt');

  readonly startDateTime = computed<Date | null>(() => {
    const date = this.formValue().date;
    const hour = this.formValue().hour;
    if (!date || hour === null || hour === undefined) return null;
    const d = new Date(date);
    d.setHours(hour, 0, 0, 0);
    return d;
  });

  readonly friendsShortBy = computed(() => {
    const need = this.minInvites();
    if (need === 0) return 0;
    return Math.max(0, need - this.friends().length);
  });

  readonly canSubmit = computed(() => {
    if (this.submitting() || this.loading()) return false;
    if (!this.field()) return false;
    if (this.startDateTime() === null) return false;
    if (this.startDateTime()!.getTime() <= Date.now()) return false;
    if (!this.invitesValid()) return false;
    return true;
  });

  constructor() {
    this.form.controls.bookingType.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.form.controls.invitedPlayerIds.setValue([]));

    this.load();
  }

  async load(): Promise<void> {
    const idParam = this.route.snapshot.paramMap.get('fieldId');
    const id = Number(idParam);
    if (!idParam || Number.isNaN(id)) {
      this.loadError.set('Invalid field.');
      this.loading.set(false);
      return;
    }

    this.loading.set(true);
    try {
      const [field, friends] = await Promise.all([
        this.fields.getById(id),
        this.friendships.getMyFriends(),
      ]);
      this.field.set(field);
      this.friends.set(friends);

      const initialType: BookingType = field.sportType === 'Basketball' ? 'FullCourt' : 'Standard';
      this.form.controls.bookingType.setValue(initialType);
    } catch {
      this.loadError.set('Could not load this field. It may have been removed.');
    } finally {
      this.loading.set(false);
    }
  }

  avatarUrl(url: string | null): string | null {
    if (!url) return null;
    if (url.startsWith('http')) return url;
    return `${environment.apiBaseUrl}${url.startsWith('/') ? url : '/' + url}`;
  }

  initials(fullName: string | null | undefined): string {
    const name = (fullName ?? '').trim();
    if (!name) return '?';
    const parts = name.split(/\s+/);
    const first = parts[0]?.[0] ?? '';
    const last = parts.length > 1 ? parts[parts.length - 1][0] : '';
    return (first + last).toUpperCase();
  }

  pickHour(h: number): void {
    this.form.controls.hour.setValue(h);
  }

  isInvited(userId: number): boolean {
    return this.form.controls.invitedPlayerIds.value.includes(userId);
  }

  toggleInvite(userId: number): void {
    const current = this.form.controls.invitedPlayerIds.value;
    if (current.includes(userId)) {
      this.form.controls.invitedPlayerIds.setValue(current.filter(id => id !== userId));
    } else {
      if (!this.canAddMoreInvites()) return;
      this.form.controls.invitedPlayerIds.setValue([...current, userId]);
    }
  }

  hourLabel(h: number): string {
    return `${h.toString().padStart(2, '0')}:00`;
  }

  async submit(): Promise<void> {
    if (!this.canSubmit()) return;

    this.submitting.set(true);
    try {
      const v = this.form.getRawValue();
      const startDateTime = this.startDateTime()!;
      const fieldId = this.field()!.id;

      await this.bookings.create({
        fieldId,
        startDate: startDateTime.toISOString(),
        bookingType: v.bookingType,
        invitedPlayersIds: v.invitedPlayerIds,
      });

      this.toast.add({
        severity: 'success',
        summary: 'Booking requested',
        detail: 'Invites have been sent to your friends.',
      });

      this.router.navigate(['/bookings']);
    } finally {
      this.submitting.set(false);
    }
  }
}
