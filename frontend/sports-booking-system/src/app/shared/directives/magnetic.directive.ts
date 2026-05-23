import {
  Directive, ElementRef, HostListener, inject, input, PLATFORM_ID, OnInit
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

/**
 * Magnetic — pulls the host toward the cursor on hover.
 * Use on buttons / interactive icons. Use the `pchMagneticStrength` input
 * to tune (0.2 subtle, 0.5 strong).
 */
@Directive({
  selector: '[pchMagnetic]',
  standalone: true
})
export class MagneticDirective implements OnInit {
  private el = inject(ElementRef<HTMLElement>);
  private platformId = inject(PLATFORM_ID);

  pchMagneticStrength = input(0.3);

  private isReducedMotion = false;

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    this.isReducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false;

    const host = this.el.nativeElement;
    host.style.transition = 'transform 260ms cubic-bezier(0.22, 1, 0.36, 1)';
    host.style.willChange = 'transform';
  }

  @HostListener('mousemove', ['$event'])
  onMove(e: MouseEvent): void {
    if (this.isReducedMotion) return;
    const host = this.el.nativeElement;
    const rect = host.getBoundingClientRect();
    const x = e.clientX - rect.left - rect.width / 2;
    const y = e.clientY - rect.top - rect.height / 2;
    const strength = this.pchMagneticStrength();
    host.style.transform = `translate(${x * strength}px, ${y * strength}px)`;
  }

  @HostListener('mouseleave')
  onLeave(): void {
    this.el.nativeElement.style.transform = 'translate(0, 0)';
  }
}
