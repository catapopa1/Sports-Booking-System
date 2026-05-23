import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { NgClass } from '@angular/common';

export type SportName = 'Football' | 'Tennis' | 'Basketball' | string;

/**
 * <pch-sport-ball sport="Football" [size]="80" [floating]="true" />
 *
 * CSS sphere with SVG markings overlay. No images, no deps.
 */
@Component({
  selector: 'pch-sport-ball',
  standalone: true,
  imports: [NgClass],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <span class="sport-ball"
          [ngClass]="ballClass()"
          [class.is-floating]="floating()"
          [style.--ball-size.px]="size()"
          [attr.aria-hidden]="true">
      @if (variant() === 'football') {
        <svg viewBox="0 0 64 64" class="markings">
          <polygon points="32,18 38,22 36,29 28,29 26,22" />
          <polygon points="44,28 48,34 44,40 38,38 39,32" />
          <polygon points="20,28 25,32 26,38 20,40 16,34" />
          <line x1="32" y1="29" x2="36" y2="22" />
          <line x1="32" y1="29" x2="28" y2="22" />
          <line x1="32" y1="29" x2="39" y2="32" />
          <line x1="32" y1="29" x2="26" y2="32" />
        </svg>
      }
      @if (variant() === 'tennis') {
        <svg viewBox="0 0 64 64" class="markings">
          <path d="M 8 30 Q 18 12, 38 14 Q 52 18, 56 36" />
          <path d="M 56 36 Q 46 52, 26 50 Q 12 46, 8 30" />
        </svg>
      }
      @if (variant() === 'basketball') {
        <svg viewBox="0 0 64 64" class="markings">
          <ellipse cx="32" cy="32" rx="24" ry="24" />
          <line x1="8" y1="32" x2="56" y2="32" />
          <path d="M 32 8 Q 22 32, 32 56" />
          <path d="M 32 8 Q 42 32, 32 56" />
        </svg>
      }
    </span>
  `,
  styles: [`
    :host { display: inline-flex; }
    .markings {
      position: absolute;
      inset: 0;
      width: 100%;
      height: 100%;
      pointer-events: none;
      stroke-width: 1.4;
      stroke-linecap: round;
      stroke-linejoin: round;
      fill: none;
      stroke: rgba(0, 0, 0, 0.45);
    }
    .sport-ball--football .markings  { stroke: rgba(0, 0, 0, 0.55); fill: rgba(0, 0, 0, 0.65); }
    .sport-ball--tennis .markings    { stroke: rgba(255, 255, 255, 0.85); stroke-width: 1.6; }
    .sport-ball--basketball .markings { stroke: rgba(40, 18, 6, 0.7); stroke-width: 1.4; }
  `]
})
export class SportBallComponent {
  sport = input<SportName>('Football');
  size = input<number>(64);
  floating = input<boolean>(false);

  variant = computed<'football' | 'tennis' | 'basketball'>(() => {
    const s = (this.sport() || '').toLowerCase();
    if (s.startsWith('tennis')) return 'tennis';
    if (s.startsWith('basket')) return 'basketball';
    return 'football';
  });

  ballClass = computed(() => `sport-ball--${this.variant()}`);
}
