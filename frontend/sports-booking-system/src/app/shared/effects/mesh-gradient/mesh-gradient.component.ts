import {
  AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, OnDestroy,
  PLATFORM_ID, ViewChild, effect, inject, input, untracked
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import {
  ShaderMount,
  meshGradientFragmentShader,
  meshGradientMeta,
  getShaderColorFromString,
  defaultPatternSizing
} from '@paper-design/shaders';

/**
 * <pch-mesh-gradient
 *    [colors]="['#181C24', '#10B981', '#87CEEB', '#232830']"
 *    [speed]="0.6"
 *    [distortion]="0.8"
 *    [swirl]="0.1"
 *    [grainOverlay]="0.05"
 * />
 *
 * Animated mesh-gradient WebGL background from Paper Design Shaders.
 * Pauses automatically when tab is hidden (handled by ShaderMount).
 */
@Component({
  selector: 'pch-mesh-gradient',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div #mount class="pch-shader-host" [class.is-rounded]="rounded()"></div>`,
  styles: [`
    :host {
      display: block;
      width: 100%;
      height: 100%;
      position: relative;
    }
    .pch-shader-host {
      width: 100%;
      height: 100%;
      overflow: hidden;
    }
    .pch-shader-host.is-rounded { border-radius: inherit; }
    .pch-shader-host > canvas { display: block; width: 100%; height: 100%; }
  `]
})
export class MeshGradientComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mount', { static: true }) mountRef!: ElementRef<HTMLDivElement>;

  private platformId = inject(PLATFORM_ID);
  private mount?: ShaderMount;

  colors = input<string[]>(['#181C24', '#10B981', '#87CEEB', '#232830']);
  speed = input<number>(0.6);
  distortion = input<number>(0.8);
  swirl = input<number>(0.1);
  grainMixer = input<number>(0);
  grainOverlay = input<number>(0.05);
  scale = input<number>(1);
  rotation = input<number>(0);
  rounded = input<boolean>(false);

  constructor() {
    effect(() => {
      // Re-read inputs to register dependencies, then update uniforms if mounted
      const colors = this.colors();
      const distortion = this.distortion();
      const swirl = this.swirl();
      const grainMixer = this.grainMixer();
      const grainOverlay = this.grainOverlay();
      const scale = this.scale();
      const rotation = this.rotation();
      const speed = this.speed();

      untracked(() => {
        if (!this.mount) return;
        this.mount.setSpeed(speed);
        this.mount.setUniforms(this.buildUniforms(colors, distortion, swirl, grainMixer, grainOverlay, scale, rotation));
      });
    });
  }

  ngAfterViewInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const host = this.mountRef.nativeElement;

    this.mount = new ShaderMount(
      host,
      meshGradientFragmentShader,
      this.buildUniforms(
        this.colors(),
        this.distortion(),
        this.swirl(),
        this.grainMixer(),
        this.grainOverlay(),
        this.scale(),
        this.rotation()
      ),
      undefined,
      this.speed()
    );
  }

  private buildUniforms(
    colors: string[],
    distortion: number,
    swirl: number,
    grainMixer: number,
    grainOverlay: number,
    scale: number,
    rotation: number
  ) {
    const maxCount = meshGradientMeta.maxColorCount;
    const clamped = colors.slice(0, maxCount);
    const padded = [...clamped];
    while (padded.length < maxCount) padded.push('#00000000');

    return {
      // Pattern sizing defaults (cover, no rotation, scale 1)
      u_fit:         2 /* cover */,
      u_scale:       scale,
      u_rotation:    rotation,
      u_originX:     defaultPatternSizing.originX,
      u_originY:     defaultPatternSizing.originY,
      u_offsetX:     defaultPatternSizing.offsetX,
      u_offsetY:     defaultPatternSizing.offsetY,
      u_worldWidth:  defaultPatternSizing.worldWidth,
      u_worldHeight: defaultPatternSizing.worldHeight,

      // Mesh-specific
      u_colors:       padded.map(c => getShaderColorFromString(c)) as number[][],
      u_colorsCount:  clamped.length,
      u_distortion:   distortion,
      u_swirl:        swirl,
      u_grainMixer:   grainMixer,
      u_grainOverlay: grainOverlay
    };
  }

  ngOnDestroy(): void {
    this.mount?.dispose();
  }
}
