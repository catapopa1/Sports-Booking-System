import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';

export type SportName = 'Football' | 'Tennis' | 'Basketball' | string;

/**
 * <pch-field-schematic sport="Tennis" [opacity]="0.12" tint="var(--emerald-500)" />
 *
 * Inline-SVG line art of the sport's field. Used as a hero watermark.
 */
@Component({
  selector: 'pch-field-schematic',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <svg class="field-schematic"
         [attr.viewBox]="viewBox()"
         [style.--schematic-opacity]="opacity()"
         [style.color]="tint()"
         preserveAspectRatio="xMidYMid meet"
         aria-hidden="true">
      @if (variant() === 'football') {
        <!-- Football pitch -->
        <rect x="4" y="4" width="392" height="232" rx="2" />
        <line x1="200" y1="4" x2="200" y2="236" />
        <circle cx="200" cy="120" r="34" />
        <circle cx="200" cy="120" r="2" />
        <!-- Left penalty area -->
        <rect x="4" y="50" width="60" height="140" />
        <rect x="4" y="84" width="22" height="72" />
        <circle cx="56" cy="120" r="2" />
        <!-- Right penalty area -->
        <rect x="336" y="50" width="60" height="140" />
        <rect x="374" y="84" width="22" height="72" />
        <circle cx="344" cy="120" r="2" />
        <!-- Corner arcs -->
        <path d="M4 14 A 10 10 0 0 1 14 4" />
        <path d="M386 4 A 10 10 0 0 1 396 14" />
        <path d="M4 226 A 10 10 0 0 0 14 236" />
        <path d="M386 236 A 10 10 0 0 0 396 226" />
      }
      @if (variant() === 'tennis') {
        <!-- Tennis court -->
        <rect x="4" y="4" width="392" height="232" rx="1" />
        <line x1="200" y1="4" x2="200" y2="236" stroke-dasharray="6 4" />
        <!-- Service boxes -->
        <line x1="56" y1="58" x2="344" y2="58" />
        <line x1="56" y1="182" x2="344" y2="182" />
        <line x1="56" y1="58" x2="56" y2="182" />
        <line x1="344" y1="58" x2="344" y2="182" />
        <line x1="200" y1="58" x2="200" y2="182" />
        <!-- Singles sidelines -->
        <line x1="38" y1="4" x2="38" y2="236" />
        <line x1="362" y1="4" x2="362" y2="236" />
      }
      @if (variant() === 'basketball') {
        <!-- Basketball half-court -->
        <rect x="4" y="4" width="392" height="232" rx="2" />
        <!-- Mid line + center circle -->
        <line x1="4" y1="120" x2="396" y2="120" />
        <circle cx="200" cy="120" r="28" />
        <!-- Left key + arc -->
        <rect x="4" y="80" width="58" height="80" />
        <circle cx="62" cy="120" r="22" />
        <path d="M4 36 Q 70 120 4 204" />
        <!-- Right key + arc -->
        <rect x="338" y="80" width="58" height="80" />
        <circle cx="338" cy="120" r="22" />
        <path d="M396 36 Q 330 120 396 204" />
      }
    </svg>
  `,
  styles: [`
    :host { display: block; width: 100%; height: 100%; }
  `]
})
export class FieldSchematicComponent {
  sport = input<SportName>('Football');
  opacity = input<number>(0.12);
  tint = input<string>('var(--emerald-500)');

  variant = computed<'football' | 'tennis' | 'basketball'>(() => {
    const s = (this.sport() || '').toLowerCase();
    if (s.startsWith('tennis')) return 'tennis';
    if (s.startsWith('basket')) return 'basketball';
    return 'football';
  });

  viewBox = computed(() => '0 0 400 240');
}
