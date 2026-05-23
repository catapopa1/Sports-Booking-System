import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { GalleriaModule } from 'primeng/galleria';
import { SkeletonModule } from 'primeng/skeleton';
import { TooltipModule } from 'primeng/tooltip';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { environment } from '../../../../environments/environment';
import { AuthService } from '../../../core/services/auth.service';
import { ParksService } from '../../../core/services/parks.service';
import { ParkDto, FieldDto } from '../../../core/models/park.models';

import { MeshGradientComponent } from '../../../shared/effects/mesh-gradient/mesh-gradient.component';
import { SpotlightCardComponent } from '../../../shared/effects/spotlight-card/spotlight-card.component';
import { ShimmerButtonComponent } from '../../../shared/effects/shimmer-button/shimmer-button.component';
import { SportBall3DComponent } from '../../../shared/components/sport-ball-3d/sport-ball-3d.component';
import { FieldSchematicComponent } from '../../../shared/components/field-schematic/field-schematic.component';
import { CountUpComponent } from '../../../shared/components/count-up/count-up.component';
import { GradientTextComponent } from '../../../shared/components/gradient-text/gradient-text.component';
import { RevealOnScrollDirective } from '../../../shared/directives/reveal-on-scroll.directive';
import { MagneticDirective } from '../../../shared/directives/magnetic.directive';

@Component({
  selector: 'app-park-detail',
  standalone: true,
  imports: [
    RouterLink, ButtonModule, SkeletonModule, TooltipModule, GalleriaModule, ConfirmDialogModule,
    MeshGradientComponent, SpotlightCardComponent, ShimmerButtonComponent,
    SportBall3DComponent, FieldSchematicComponent, CountUpComponent, GradientTextComponent,
    RevealOnScrollDirective, MagneticDirective
  ],
  providers: [ConfirmationService],
  templateUrl: './park-detail.html',
  styleUrl: './park-detail.scss',
})
export class ParkDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly parksService = inject(ParksService);
  private readonly auth = inject(AuthService);
  private readonly toast = inject(MessageService);
  private readonly confirm = inject(ConfirmationService);

  readonly park = signal<ParkDto | null>(null);
  readonly fields = signal<FieldDto[]>([]);
  readonly loading = signal<boolean>(true);
  readonly error = signal<string | null>(null);
  readonly uploadingPhoto = signal<boolean>(false);
  readonly photoBusyId = signal<number | null>(null);

  readonly lightboxVisible = signal<boolean>(false);
  readonly lightboxIndex = signal<number>(0);
  readonly photosExpanded = signal<boolean>(false);

  readonly PHOTOS_COLLAPSED_LIMIT = 4;

  readonly visiblePhotos = computed(() => {
    const photos = this.park()?.photos ?? [];
    return this.photosExpanded() ? photos : photos.slice(0, this.PHOTOS_COLLAPSED_LIMIT);
  });

  readonly canManagePhotos = computed(() => {
    const p = this.park();
    if (!p) return false;
    return this.auth.role() === 'Admin' || p.managerId === this.auth.userId();
  });

  openLightbox(index: number): void {
    this.lightboxIndex.set(index);
    this.lightboxVisible.set(true);
  }

  togglePhotosExpanded(): void {
    this.photosExpanded.update(v => !v);
  }

  /** Pick the most-common sport in the park's fields — used to flavour the hero watermark. */
  readonly dominantSport = computed<string>(() => {
    const counts = new Map<string, number>();
    for (const f of this.fields()) {
      counts.set(f.sportType, (counts.get(f.sportType) ?? 0) + 1);
    }
    let top = 'Football';
    let max = 0;
    for (const [sport, c] of counts.entries()) {
      if (c > max) { max = c; top = sport; }
    }
    return top;
  });

  /** Mesh gradient colors — varies subtly with dominant sport.
   *  Base = carbon-900 + carbon-800, accent = emerald + sky for that
   *  cohesive "pitch under stadium light" feel. */
  readonly heroColors = computed<string[]>(() => {
    const sport = this.dominantSport();
    if (sport === 'Tennis')     return ['#181C24', '#10B981', '#87CEEB', '#F4C430'];
    if (sport === 'Basketball') return ['#181C24', '#10B981', '#87CEEB', '#FF6B6B'];
    return                              ['#181C24', '#10B981', '#87CEEB', '#232830'];
  });

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    const id = Number(idParam);
    if (!idParam || Number.isNaN(id)) {
      this.error.set('Invalid park id.');
      this.loading.set(false);
      return;
    }
    this.load(id);
  }

  async load(id: number): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const [park, fields] = await Promise.all([
        this.parksService.getById(id),
        this.parksService.getFields(id),
      ]);
      this.park.set(park);
      this.fields.set(fields);
    } catch {
      this.error.set('Failed to load this park. It may have been removed.');
    } finally {
      this.loading.set(false);
    }
  }

  absoluteUrl(url: string | null | undefined): string | null {
    if (!url) return null;
    if (url.startsWith('http')) return url;
    return `${environment.apiBaseUrl}${url.startsWith('/') ? url : '/' + url}`;
  }

  async onPhotoSelected(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    const park = this.park();
    if (!file || !park) return;

    if (!file.type.startsWith('image/')) {
      this.toast.add({ severity: 'warn', summary: 'Invalid file', detail: 'Please choose an image file.' });
      input.value = '';
      return;
    }

    this.uploadingPhoto.set(true);
    try {
      await this.parksService.uploadPhoto(park.id, file);
      this.toast.add({ severity: 'success', summary: 'Photo uploaded' });
      await this.load(park.id);
    } finally {
      this.uploadingPhoto.set(false);
      input.value = '';
    }
  }

  async setMain(photoId: number): Promise<void> {
    const park = this.park();
    if (!park) return;
    this.photoBusyId.set(photoId);
    try {
      await this.parksService.setMainPhoto(park.id, photoId);
      this.toast.add({ severity: 'success', summary: 'Main photo updated' });
      await this.load(park.id);
    } finally {
      this.photoBusyId.set(null);
    }
  }

  deletePhoto(photoId: number): void {
    const park = this.park();
    if (!park) return;
    this.confirm.confirm({
      header: 'Delete photo',
      message: 'Remove this photo from the gallery? This cannot be undone.',
      acceptLabel: 'Delete',
      rejectLabel: 'Cancel',
      acceptButtonStyleClass: 'p-button-danger',
      accept: async () => {
        this.photoBusyId.set(photoId);
        try {
          await this.parksService.deletePhoto(park.id, photoId);
          this.toast.add({ severity: 'success', summary: 'Photo deleted' });
          await this.load(park.id);
        } finally {
          this.photoBusyId.set(null);
        }
      },
    });
  }
}
