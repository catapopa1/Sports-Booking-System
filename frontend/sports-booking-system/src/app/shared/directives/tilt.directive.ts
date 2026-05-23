import {
  Directive, ElementRef, HostListener, inject, input, OnInit, PLATFORM_ID
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

/**
 * 3D Tilt — applies rotateX/Y on mousemove with perspective for a subtle parallax.
 * Inspired by Aceternity's 3D Card. The host should be `transform-style: preserve-3d`
 * if you want children to participate.
 */
@Directive({
  selector: '[pchTilt]',
  standalone: true
})
export class TiltDirective implements OnInit {
  private el = inject(ElementRef<HTMLElement>);
  private platformId = inject(PLATFORM_ID);

  /** Max rotation in degrees */
  pchTiltMax = input(8);
  /** Perspective in px */
  pchTiltPerspective = input(1000);
  /** Scale on hover */
  pchTiltScale = input(1.02);

  private isReducedMotion = false;

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    this.isReducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false;

    const host = this.el.nativeElement;
    host.style.transition = 'transform 220ms cubic-bezier(0.22, 1, 0.36, 1)';
    host.style.willChange = 'transform';
  }

  @HostListener('mousemove', ['$event'])
  onMove(e: MouseEvent): void {
    if (this.isReducedMotion) return;
    const host = this.el.nativeElement;
    const rect = host.getBoundingClientRect();
    const max = this.pchTiltMax();
    const rotY = (((e.clientX - rect.left) / rect.width) - 0.5) * (max * 2);
    const rotX = -(((e.clientY - rect.top) / rect.height) - 0.5) * (max * 2);
    host.style.transform =
      `perspective(${this.pchTiltPerspective()}px) ` +
      `rotateX(${rotX}deg) rotateY(${rotY}deg) ` +
      `scale(${this.pchTiltScale()})`;
  }

  @HostListener('mouseleave')
  onLeave(): void {
    this.el.nativeElement.style.transform =
      `perspective(${this.pchTiltPerspective()}px) rotateX(0) rotateY(0) scale(1)`;
  }
}
