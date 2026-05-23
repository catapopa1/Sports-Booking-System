import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/**
 * Port of Magic UI "Border Beam".
 * A traveling light beam that animates along the border of a card.
 * Drop inside a relatively-positioned container.
 *
 *   <div class="card" style="position: relative; overflow: hidden;">
 *     <pch-border-beam [duration]="6" [size]="160" />
 *     ...content
 *   </div>
 */
@Component({
  selector: 'pch-border-beam',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <span class="border-beam"
          [style.--beam-size.px]="size()"
          [style.--beam-duration.s]="duration()"
          [style.--beam-color-from]="colorFrom()"
          [style.--beam-color-to]="colorTo()"
          [style.--beam-delay.s]="delay()"
          aria-hidden="true"></span>
  `,
  styles: [`
    :host {
      position: absolute;
      inset: 0;
      pointer-events: none;
      border-radius: inherit;
    }
    .border-beam {
      position: absolute;
      inset: 0;
      border-radius: inherit;
      pointer-events: none;
      /* The traveling spark — a small radial-gradient rectangle that moves
         along the inner border path. */
      &::after {
        content: '';
        position: absolute;
        inset: 0;
        border-radius: inherit;
        padding: 1px;
        background:
          conic-gradient(
            from var(--beam-angle, 0deg),
            transparent 0%,
            transparent 70%,
            var(--beam-color-from, #10B981) 80%,
            var(--beam-color-to, #87CEEB) 90%,
            transparent 100%
          );
        -webkit-mask:
          linear-gradient(#000 0 0) content-box,
          linear-gradient(#000 0 0);
                mask:
          linear-gradient(#000 0 0) content-box,
          linear-gradient(#000 0 0);
        -webkit-mask-composite: xor;
                mask-composite: exclude;
        animation: beam-rotate var(--beam-duration, 6s) linear infinite;
        animation-delay: var(--beam-delay, 0s);
      }
    }
    @keyframes beam-rotate {
      to { --beam-angle: 360deg; }
    }
    @property --beam-angle {
      syntax: '<angle>';
      inherits: false;
      initial-value: 0deg;
    }
    @media (prefers-reduced-motion: reduce) {
      .border-beam::after { animation: none; }
    }
  `]
})
export class BorderBeamComponent {
  /** Beam length in px (visual width of the conic-gradient highlight) */
  size = input<number>(160);
  /** Full rotation duration in seconds */
  duration = input<number>(6);
  /** Start of the beam color */
  colorFrom = input<string>('#10B981');
  /** End of the beam color */
  colorTo = input<string>('#87CEEB');
  /** Delay in seconds before animation starts */
  delay = input<number>(0);
}
