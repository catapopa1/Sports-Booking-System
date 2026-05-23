import {
  AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, OnDestroy,
  PLATFORM_ID, ViewChild, computed, effect, inject, input, untracked
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import * as THREE from 'three';
import { textureForSport } from './sport-ball-texture';

export type SportName = 'Football' | 'Tennis' | 'Basketball' | string;

/**
 * <pch-sport-ball-3d sport="Football" [size]="96" [speed]="0.4" [interactive]="true" />
 *
 * A WebGL 3D sport ball with a procedural texture, lit and continuously
 * rotating. Each instance creates its own tiny canvas + WebGL context
 * (~80×80 px renders are cheap and look great).
 *
 * Inputs:
 *   sport       — Football | Tennis | Basketball (picks the texture)
 *   size        — render box in px (default 96)
 *   speed       — radians/second of Y rotation (default 0.5)
 *   interactive — drag to spin (default false)
 *   floating    — also apply a subtle vertical breathe via CSS (default false)
 */
@Component({
  selector: 'pch-sport-ball-3d',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div #wrap class="ball-3d-wrap"
         [class.is-floating]="floating()"
         [style.width.px]="size()"
         [style.height.px]="size()"
         [attr.aria-hidden]="true">
      <canvas #canvas></canvas>
    </div>
  `,
  styles: [`
    :host { display: inline-flex; }
    .ball-3d-wrap {
      position: relative;
      display: inline-block;
      filter: drop-shadow(0 8px 14px rgba(0, 0, 0, 0.30));
    }
    .ball-3d-wrap.is-floating { animation: breathe 4s cubic-bezier(0.65, 0, 0.35, 1) infinite; }
    canvas { display: block; width: 100%; height: 100%; }
    @media (prefers-reduced-motion: reduce) {
      .ball-3d-wrap.is-floating { animation: none; }
    }
  `]
})
export class SportBall3DComponent implements AfterViewInit, OnDestroy {
  @ViewChild('canvas', { static: true }) canvasRef!: ElementRef<HTMLCanvasElement>;

  private platformId = inject(PLATFORM_ID);

  sport = input<SportName>('Football');
  size = input<number>(96);
  speed = input<number>(0.5);
  interactive = input<boolean>(false);
  floating = input<boolean>(false);

  variant = computed<'football' | 'tennis' | 'basketball'>(() => {
    const s = (this.sport() || '').toLowerCase();
    if (s.startsWith('tennis')) return 'tennis';
    if (s.startsWith('basket')) return 'basketball';
    return 'football';
  });

  private renderer?: THREE.WebGLRenderer;
  private scene?: THREE.Scene;
  private camera?: THREE.PerspectiveCamera;
  private mesh?: THREE.Mesh;
  private texture?: THREE.CanvasTexture;
  private rafId?: number;
  private lastTime = 0;
  private isReducedMotion = false;

  // Drag state for interactive mode
  private dragging = false;
  private prevX = 0;
  private prevY = 0;
  private dragVelX = 0;
  private dragVelY = 0;

  constructor() {
    // React to sport changes — regenerate the texture
    effect(() => {
      const sport = this.sport();
      untracked(() => {
        if (!this.mesh) return;
        const newTex = new THREE.CanvasTexture(textureForSport(sport));
        newTex.colorSpace = THREE.SRGBColorSpace;
        newTex.anisotropy = 4;
        (this.mesh.material as THREE.MeshStandardMaterial).map?.dispose();
        (this.mesh.material as THREE.MeshStandardMaterial).map = newTex;
        (this.mesh.material as THREE.MeshStandardMaterial).needsUpdate = true;
        this.texture?.dispose();
        this.texture = newTex;
      });
    });

    // React to size changes — resize the renderer
    effect(() => {
      const s = this.size();
      untracked(() => this.resize(s));
    });
  }

  ngAfterViewInit(): void {
    if (!isPlatformBrowser(this.platformId)) return;
    this.isReducedMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches ?? false;
    this.init();
    if (this.interactive()) this.bindDragHandlers();
  }

  private init(): void {
    const canvas = this.canvasRef.nativeElement;
    const size = this.size();

    const renderer = new THREE.WebGLRenderer({
      canvas,
      antialias: true,
      alpha: true,
      powerPreference: 'low-power'
    });
    renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));
    renderer.setSize(size, size, false);
    renderer.outputColorSpace = THREE.SRGBColorSpace;
    this.renderer = renderer;

    const scene = new THREE.Scene();
    this.scene = scene;

    const camera = new THREE.PerspectiveCamera(35, 1, 0.1, 100);
    camera.position.set(0, 0, 4.2);
    this.camera = camera;

    // Lighting — soft ambient + a key + a volt rim for accent pop
    scene.add(new THREE.AmbientLight(0xffffff, 0.55));

    const key = new THREE.DirectionalLight(0xffffff, 1.4);
    key.position.set(2, 3, 3);
    scene.add(key);

    const rim = new THREE.DirectionalLight(0x10B981, 0.55);
    rim.position.set(-2.5, 1.5, -2);
    scene.add(rim);

    // The ball
    const texture = new THREE.CanvasTexture(textureForSport(this.sport()));
    texture.colorSpace = THREE.SRGBColorSpace;
    texture.anisotropy = 4;
    this.texture = texture;

    const geo = new THREE.SphereGeometry(1, 48, 48);
    const mat = new THREE.MeshStandardMaterial({
      map: texture,
      roughness: 0.55,
      metalness: 0.05
    });
    const mesh = new THREE.Mesh(geo, mat);
    mesh.rotation.x = 0.25;
    scene.add(mesh);
    this.mesh = mesh;

    this.lastTime = performance.now();
    this.animate();
  }

  private animate = (): void => {
    if (!this.renderer || !this.scene || !this.camera || !this.mesh) return;

    const now = performance.now();
    const dt = Math.min(0.05, (now - this.lastTime) / 1000);
    this.lastTime = now;

    if (!this.isReducedMotion) {
      if (this.dragging) {
        this.mesh.rotation.y += this.dragVelX;
        this.mesh.rotation.x += this.dragVelY;
        this.dragVelX *= 0.7;
        this.dragVelY *= 0.7;
      } else {
        // Continuous spin + decaying drag inertia
        this.mesh.rotation.y += this.speed() * dt + this.dragVelX;
        this.mesh.rotation.x += this.dragVelY;
        this.dragVelX *= 0.96;
        this.dragVelY *= 0.96;
      }
    }

    this.renderer.render(this.scene, this.camera);
    this.rafId = requestAnimationFrame(this.animate);
  };

  private resize(size: number): void {
    if (!this.renderer) return;
    this.renderer.setSize(size, size, false);
  }

  private bindDragHandlers(): void {
    const canvas = this.canvasRef.nativeElement;
    canvas.style.cursor = 'grab';

    const onDown = (e: PointerEvent) => {
      this.dragging = true;
      this.prevX = e.clientX;
      this.prevY = e.clientY;
      canvas.style.cursor = 'grabbing';
      canvas.setPointerCapture(e.pointerId);
    };
    const onMove = (e: PointerEvent) => {
      if (!this.dragging) return;
      this.dragVelX = (e.clientX - this.prevX) * 0.005;
      this.dragVelY = (e.clientY - this.prevY) * 0.005;
      this.prevX = e.clientX;
      this.prevY = e.clientY;
    };
    const onUp = () => {
      this.dragging = false;
      canvas.style.cursor = 'grab';
    };

    canvas.addEventListener('pointerdown', onDown);
    canvas.addEventListener('pointermove', onMove);
    canvas.addEventListener('pointerup', onUp);
    canvas.addEventListener('pointercancel', onUp);
  }

  ngOnDestroy(): void {
    if (this.rafId) cancelAnimationFrame(this.rafId);
    this.texture?.dispose();
    if (this.mesh) {
      this.mesh.geometry.dispose();
      (this.mesh.material as THREE.Material).dispose();
    }
    this.renderer?.dispose();
  }
}
