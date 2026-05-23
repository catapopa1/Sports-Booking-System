import { ChangeDetectionStrategy, Component, input } from '@angular/core';

/**
 * <pch-gradient-text>Hero title</pch-gradient-text>
 *
 * Wraps content with the gold→cream→gold shimmer gradient (defined in
 * styles/_glass.scss as `.gradient-text`). Use as inline element.
 */
@Component({
  selector: 'pch-gradient-text',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<span class="gradient-text" [style.animation-duration.s]="speed()"><ng-content /></span>`,
  styles: [`
    :host { display: inline; }
  `]
})
export class GradientTextComponent {
  /** Animation duration in seconds */
  speed = input<number>(6);
}
