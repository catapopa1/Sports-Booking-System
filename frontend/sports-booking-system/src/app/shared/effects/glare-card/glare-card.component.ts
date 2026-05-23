import {
  ChangeDetectionStrategy, Component, ElementRef, HostListener,
  PLATFORM_ID, inject, input, signal
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

/**
 * Port of Aceternity's Glare Card.
 * A card that tilts subtly on mousemove and shows a moving radial glare
 * highlight tracking the cursor.
 *
 *   <pch-glare-card>
 *     <ng-content sits inside />
 *   </pch-glare-card>
 */
@Component({
  selector: 'pch-glare-card',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="glare-card"
         [style.transform]="transform()"
         (mouseleave)="reset()">
      <div class="glare-card__content"><ng-content /></div>
      <div class="glare-card__glare"
           [style.background]="glareBg()"
           aria-hidden="true"></div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      perspective: 1000px;
    }
    .glare-card {
      position: relative;
      width: 100%;
      height: 100%;
      border-radius: inherit;
      transform-style: preserve-3d;
      transition: transform 280ms cubic-bezier(0.22, 1, 0.36, 1);
      overflow: hidden;
      will-change: transform;
    }
    .glare-card__content {
      position: relative;
      z-index: 1;
      width: 100%;
      height: 100%;
    }
    .glare-card__glare {
      position: absolute;
      inset: 0;
      pointer-events: none;
      mix-blend-mode: overlay;
      opacity: 0;
      transition: opacity 280ms cubic-bezier(0.22, 1, 0.36, 1);
      z-index: 2;
      border-radius: inherit;
    }
    .glare-card:hover .glare-card__glare { opacity: 0.85; }

    @media (prefers-reduced-motion: reduce) {
      .glare-card { transition: none; }
      .glare-card__glare { display: none; }
    }
  `]
})
export class GlareCardComponent {
  private host = inject(ElementRef<HTMLElement>);
  private platformId = inject(PLATFORM_ID);

  /** Max tilt in degrees */
  tilt = input<number>(6);
  /** Glare color (CSS color string) */
  glareColor = input<string>('rgba(167, 243, 208, 0.55)');

  private rotateX = signal(0);
  private rotateY = signal(0);
  private mx = signal(50);
  private my = signal(50);
  private isReducedMotion = false;

  constructor() {
    if (isPlatformBrowser(this.platformId)) {
      this.isReducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false;
    }
  }

  @HostListener('mousemove', ['$event'])
  onMove(e: MouseEvent): void {
    if (this.isReducedMotion) return;
    const rect = this.host.nativeElement.getBoundingClientRect();
    const px = (e.clientX - rect.left) / rect.width;
    const py = (e.clientY - rect.top)  / rect.height;
    const max = this.tilt();
    this.rotateX.set(-(py - 0.5) * (max * 2));
    this.rotateY.set( (px - 0.5) * (max * 2));
    this.mx.set(px * 100);
    this.my.set(py * 100);
  }

  reset(): void {
    this.rotateX.set(0);
    this.rotateY.set(0);
  }

  transform(): string {
    return `rotateX(${this.rotateX()}deg) rotateY(${this.rotateY()}deg)`;
  }

  glareBg(): string {
    return `radial-gradient(circle at ${this.mx()}% ${this.my()}%, ${this.glareColor()} 0%, transparent 60%)`;
  }
}
