import {
  Directive, ElementRef, OnInit, OnDestroy, inject, input, PLATFORM_ID
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Directive({
  selector: '[pchReveal]',
  standalone: true
})
export class RevealOnScrollDirective implements OnInit, OnDestroy {
  private el = inject(ElementRef<HTMLElement>);
  private platformId = inject(PLATFORM_ID);

  /** Animation variant — 'up' (default), 'blur', or 'fade' */
  pchReveal = input<'up' | 'blur' | 'fade'>('up');
  /** Stagger delay in ms (multiplied by index of this element among siblings) */
  pchRevealDelay = input(0);
  /** Trigger threshold (0–1) */
  pchRevealThreshold = input(0.15);

  private observer?: IntersectionObserver;

  ngOnInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    const host = this.el.nativeElement;
    const variant = this.pchReveal();
    const delay = this.pchRevealDelay();

    // Initial state
    host.style.opacity = '0';
    host.style.willChange = 'opacity, transform, filter';
    if (variant === 'up')   host.style.transform = 'translateY(24px)';
    if (variant === 'blur') {
      host.style.transform = 'translateY(12px)';
      host.style.filter = 'blur(8px)';
    }
    host.style.transition =
      `opacity 720ms cubic-bezier(0.22, 1, 0.36, 1) ${delay}ms,` +
      `transform 720ms cubic-bezier(0.22, 1, 0.36, 1) ${delay}ms,` +
      `filter 720ms cubic-bezier(0.22, 1, 0.36, 1) ${delay}ms`;

    this.observer = new IntersectionObserver((entries) => {
      for (const entry of entries) {
        if (entry.isIntersecting) {
          host.style.opacity = '1';
          host.style.transform = 'translateY(0)';
          host.style.filter = 'none';
          this.observer?.unobserve(host);
        }
      }
    }, { threshold: this.pchRevealThreshold() });

    this.observer.observe(host);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
  }
}
