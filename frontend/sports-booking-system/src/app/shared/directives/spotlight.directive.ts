import {
  Directive, ElementRef, HostBinding, HostListener, inject, OnInit
} from '@angular/core';

/**
 * Spotlight — tracks the mouse and exposes --mx / --my CSS variables on the host.
 * Pair with the `.spotlight` class from styles/_glass.scss for the radial glow.
 */
@Directive({
  selector: '[pchSpotlight]',
  standalone: true,
  host: { 'class': 'spotlight' }
})
export class SpotlightDirective {
  private el = inject(ElementRef<HTMLElement>);

  @HostListener('mousemove', ['$event'])
  onMove(e: MouseEvent): void {
    const host = this.el.nativeElement;
    const rect = host.getBoundingClientRect();
    const x = ((e.clientX - rect.left) / rect.width) * 100;
    const y = ((e.clientY - rect.top) / rect.height) * 100;
    host.style.setProperty('--mx', `${x}%`);
    host.style.setProperty('--my', `${y}%`);
  }
}
