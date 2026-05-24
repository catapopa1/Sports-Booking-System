import { Component, computed, input } from '@angular/core';

type Tone = 'pending' | 'success' | 'danger' | 'warning' | 'neutral';

const TONE_BY_STATUS: Record<string, Tone> = {
  Requested:                  'pending',
  PendingPlayerConfirmations: 'pending',
  PendingManagerApproval:     'pending',
  Pending:                    'pending',
  Confirmed:                  'success',
  Accepted:                   'success',
  Cancelled:                  'danger',
  Declined:                   'danger',
  TimedOut:                   'warning',
};

const LABEL_BY_STATUS: Record<string, string> = {
  Requested:                  'Requested',
  PendingPlayerConfirmations: 'Awaiting players',
  PendingManagerApproval:     'Awaiting approval',
  Pending:                    'Pending',
  Confirmed:                  'Confirmed',
  Accepted:                   'Accepted',
  Cancelled:                  'Cancelled',
  Declined:                   'Declined',
  TimedOut:                   'Timed out',
};

@Component({
  selector: 'app-status-chip',
  standalone: true,
  templateUrl: './status-chip.html',
  styleUrl: './status-chip.scss',
})
export class StatusChipComponent {
  readonly status = input.required<string>();

  readonly tone = computed<Tone>(() => TONE_BY_STATUS[this.status()] ?? 'neutral');
  readonly label = computed<string>(() => LABEL_BY_STATUS[this.status()] ?? this.status());
}
