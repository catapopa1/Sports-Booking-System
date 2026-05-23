import {
  ChangeDetectionStrategy, Component, ElementRef, OnDestroy,
  PLATFORM_ID, computed, effect, inject, input, signal, untracked
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

/**
 * <pch-count-up [value]="42.5" [decimals]="2" prefix="$" suffix=" / hr" [duration]="1200" />
 *
 * Animates from 0 (or the previous value) to the target when scrolled into view.
 * Respects prefers-reduced-motion (jumps directly).
 */
@Component({
  selector: 'pch-count-up',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<span class="tabular">{{ prefix() }}{{ display() }}{{ suffix() }}</span>`,
  styles: [`
    :host {
      display: inline-block;
      font-variant-numeric: tabular-nums;
      font-feature-settings: 'tnum';
    }
  `]
})
export class CountUpComponent implements OnDestroy {
  private host = inject(ElementRef<HTMLElement>);
  private platformId = inject(PLATFORM_ID);

  value = input<number>(0);
  decimals = input<number>(0);
  prefix = input<string>('');
  suffix = input<string>('');
  duration = input<number>(1100);

  private current = signal(0);
  private observer?: IntersectionObserver;
  private rafId?: number;
  private hasAnimated = false;

  display = computed(() => {
    const n = this.current();
    return n.toLocaleString(undefined, {
      minimumFractionDigits: this.decimals(),
      maximumFractionDigits: this.decimals()
    });
  });

  constructor() {
    effect(() => {
      const target = this.value();
      if (!isPlatformBrowser(this.platformId)) {
        untracked(() => this.current.set(target));
        return;
      }

      if (!this.hasAnimated) {
        this.observer?.disconnect();
        this.observer = new IntersectionObserver((entries) => {
          for (const e of entries) {
            if (e.isIntersecting) {
              this.observer?.disconnect();
              this.animateTo(target);
              this.hasAnimated = true;
            }
          }
        }, { threshold: 0.25 });
        this.observer.observe(this.host.nativeElement);
      } else {
        this.animateTo(target);
      }
    });
  }

  private animateTo(target: number): void {
    const reduced = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false;
    if (reduced) {
      this.current.set(target);
      return;
    }

    const start = this.current();
    const startTime = performance.now();
    const dur = this.duration();
    const ease = (t: number) => 1 - Math.pow(1 - t, 4); // ease-out-quart

    const step = (now: number) => {
      const t = Math.min(1, (now - startTime) / dur);
      this.current.set(start + (target - start) * ease(t));
      if (t < 1) this.rafId = requestAnimationFrame(step);
    };
    this.rafId = requestAnimationFrame(step);
  }

  ngOnDestroy(): void {
    this.observer?.disconnect();
    if (this.rafId) cancelAnimationFrame(this.rafId);
  }
}
