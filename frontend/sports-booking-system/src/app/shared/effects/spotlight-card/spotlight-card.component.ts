import {
  ChangeDetectionStrategy, Component, ElementRef, HostListener, inject
} from '@angular/core';

/**
 * Port of Aceternity's Spotlight Card.
 * The card tracks the mouse and reveals a soft radial highlight.
 *
 *   <pch-spotlight-card class="my-card"> ... children ... </pch-spotlight-card>
 *
 * Implementation note: we do NOT wrap <ng-content /> in a div — that would
 * defeat any flex/grid layout the parent applies to this host. Instead, the
 * spotlight glow is drawn entirely via the ::before pseudo on the host.
 */
@Component({
  selector: 'pch-spotlight-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<ng-content />`,
  styles: [`
    :host {
      position: relative;
      isolation: isolate;
      overflow: hidden;
      /* No display rule here — host inherits from parent's class.
         If you want it as a flex card, add display: flex on the host class. */
    }
    :host::before {
      content: '';
      position: absolute;
      inset: -1px;
      pointer-events: none;
      background:
        radial-gradient(
          480px circle at var(--mx, 50%) var(--my, 50%),
          var(--spotlight-color, rgba(16, 185, 129, 0.20)),
          transparent 60%
        );
      opacity: 0;
      transition: opacity 280ms cubic-bezier(0.22, 1, 0.36, 1);
      z-index: 0;
      border-radius: inherit;
    }
    :host:hover::before { opacity: 1; }
  `]
})
export class SpotlightCardComponent {
  private host = inject(ElementRef<HTMLElement>);

  @HostListener('mousemove', ['$event'])
  onMove(e: MouseEvent): void {
    const host = this.host.nativeElement;
    const rect = host.getBoundingClientRect();
    const x = ((e.clientX - rect.left) / rect.width) * 100;
    const y = ((e.clientY - rect.top)  / rect.height) * 100;
    host.style.setProperty('--mx', `${x}%`);
    host.style.setProperty('--my', `${y}%`);
  }
}
