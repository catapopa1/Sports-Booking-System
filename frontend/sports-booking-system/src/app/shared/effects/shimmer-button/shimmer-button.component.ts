import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/**
 * Port of Magic UI "Shimmer Button".
 * A button with a continuous metallic shimmer that travels around the edge.
 * The shimmer is produced by a conic-gradient that rotates on an inner
 * pseudo-layer, with a solid 1px-inset background mask on top so the gradient
 * only shows as a thin ring around the edge.
 *
 *   <button pch-shimmer-button>Book this court</button>
 *   <a pch-shimmer-button href="...">Continue</a>
 *
 * Variants: 'solid' (gold filled, default) | 'outlined' (translucent on dark)
 */
@Component({
  selector: 'button[pch-shimmer-button], a[pch-shimmer-button]',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <span class="shimmer-ring"
          aria-hidden="true"
          [style.--shimmer-duration.s]="duration()"></span>
    <span class="shimmer-fill" aria-hidden="true"></span>
    <span class="shimmer-content"><ng-content /></span>
  `,
  host: {
    '[class.shimmer-btn]': 'true',
    '[attr.data-variant]': 'variant()'
  },
  styles: [`
    :host {
      position: relative;
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 0.5rem;
      padding: 0.75rem 1.5rem;
      border-radius: 999px;
      border: none;
      font-family: inherit;
      font-weight: 600;
      font-size: 0.9375rem;
      letter-spacing: 0.01em;
      line-height: 1;
      isolation: isolate;
      overflow: hidden;
      cursor: pointer;
      text-decoration: none;
      background: transparent;
      transition: transform 200ms cubic-bezier(0.22, 1, 0.36, 1),
                  box-shadow 200ms cubic-bezier(0.22, 1, 0.36, 1);
      will-change: transform;
    }
    :host[data-variant="solid"]    { color: #FFFFFF; }
    :host[data-variant="outlined"] { color: #6EE7B7; }

    :host:not(:disabled):hover  { transform: translateY(-1px); box-shadow: 0 10px 26px rgba(16, 185, 129, 0.40); }
    :host:not(:disabled):active { transform: translateY(0); }
    :host:disabled              { opacity: 0.5; cursor: not-allowed; }

    /* Layer 0 (back) — the rotating conic-gradient (emerald with white peak) */
    .shimmer-ring {
      position: absolute;
      inset: 0;
      border-radius: inherit;
      z-index: 0;
      background: conic-gradient(
        from var(--shimmer-angle, 0deg),
        transparent 0deg,
        rgba(255, 255, 255, 0.95) 20deg,
        rgba(167, 243, 208, 1) 35deg,
        rgba(255, 255, 255, 0.95) 50deg,
        transparent 70deg,
        transparent 360deg
      );
      animation: shimmer-rotate var(--shimmer-duration, 2.4s) linear infinite;
    }

    /* Layer 1 (mid) — the solid button face, inset 1.5px to leave a ring */
    .shimmer-fill {
      position: absolute;
      inset: 1.5px;
      border-radius: inherit;
      z-index: 1;
    }
    :host[data-variant="solid"]    .shimmer-fill { background: linear-gradient(180deg, #34D399 0%, #10B981 100%); }
    :host[data-variant="outlined"] .shimmer-fill { background: rgba(24, 28, 36, 0.78); backdrop-filter: blur(8px); }

    /* Layer 2 (top) — actual content */
    .shimmer-content {
      position: relative;
      z-index: 2;
      display: inline-flex;
      align-items: center;
      gap: 0.5rem;
    }

    @keyframes shimmer-rotate {
      to { --shimmer-angle: 360deg; }
    }
    @property --shimmer-angle {
      syntax: '<angle>';
      inherits: false;
      initial-value: 0deg;
    }
    @media (prefers-reduced-motion: reduce) {
      .shimmer-ring { animation: none; }
    }
  `]
})
export class ShimmerButtonComponent {
  variant = input<'solid' | 'outlined'>('solid');
  duration = input<number>(2.4);
}
