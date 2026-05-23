import {
  AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, OnDestroy,
  PLATFORM_ID, ViewChild, effect, inject, input, untracked
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import {
  ShaderMount,
  dotOrbitFragmentShader,
  dotOrbitMeta,
  getShaderColorFromString,
  defaultPatternSizing
} from '@paper-design/shaders';

@Component({
  selector: 'pch-dot-orbit',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `<div #mount class="pch-shader-host"></div>`,
  styles: [`
    :host { display: block; width: 100%; height: 100%; position: relative; }
    .pch-shader-host { width: 100%; height: 100%; overflow: hidden; }
    .pch-shader-host > canvas { display: block; width: 100%; height: 100%; }
  `]
})
export class DotOrbitComponent implements AfterViewInit, OnDestroy {
  @ViewChild('mount', { static: true }) mountRef!: ElementRef<HTMLDivElement>;

  private platformId = inject(PLATFORM_ID);
  private mount?: ShaderMount;

  colorBack = input<string>('#00000000');
  colors = input<string[]>(['#10B981', '#87CEEB']);
  speed = input<number>(0.6);
  size = input<number>(0.18);
  sizeRange = input<number>(0.6);
  spreading = input<number>(0.55);
  stepsPerColor = input<number>(1);
  scale = input<number>(1);
  rotation = input<number>(0);

  constructor() {
    effect(() => {
      const colorBack = this.colorBack();
      const colors = this.colors();
      const size = this.size();
      const sizeRange = this.sizeRange();
      const spreading = this.spreading();
      const stepsPerColor = this.stepsPerColor();
      const scale = this.scale();
      const rotation = this.rotation();
      const speed = this.speed();

      untracked(() => {
        if (!this.mount) return;
        this.mount.setSpeed(speed);
        this.mount.setUniforms(this.buildUniforms(
          colorBack, colors, size, sizeRange, spreading, stepsPerColor, scale, rotation
        ));
      });
    });
  }

  ngAfterViewInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    const host = this.mountRef.nativeElement;

    this.mount = new ShaderMount(
      host,
      dotOrbitFragmentShader,
      this.buildUniforms(
        this.colorBack(),
        this.colors(),
        this.size(),
        this.sizeRange(),
        this.spreading(),
        this.stepsPerColor(),
        this.scale(),
        this.rotation()
      ),
      undefined,
      this.speed()
    );
  }

  private buildUniforms(
    colorBack: string,
    colors: string[],
    size: number,
    sizeRange: number,
    spreading: number,
    stepsPerColor: number,
    scale: number,
    rotation: number
  ) {
    const maxCount = dotOrbitMeta.maxColorCount;
    const clamped = colors.slice(0, maxCount);
    const padded = [...clamped];
    while (padded.length < maxCount) padded.push('#00000000');

    return {
      u_fit:         2 /* cover */,
      u_scale:       scale,
      u_rotation:    rotation,
      u_originX:     defaultPatternSizing.originX,
      u_originY:     defaultPatternSizing.originY,
      u_offsetX:     defaultPatternSizing.offsetX,
      u_offsetY:     defaultPatternSizing.offsetY,
      u_worldWidth:  defaultPatternSizing.worldWidth,
      u_worldHeight: defaultPatternSizing.worldHeight,

      u_colorBack:     getShaderColorFromString(colorBack) as number[],
      u_colors:        padded.map(c => getShaderColorFromString(c)) as number[][],
      u_colorsCount:   clamped.length,
      u_size:          size,
      u_sizeRange:     sizeRange,
      u_spreading:     spreading,
      u_stepsPerColor: stepsPerColor
    };
  }

  ngOnDestroy(): void {
    this.mount?.dispose();
  }
}
