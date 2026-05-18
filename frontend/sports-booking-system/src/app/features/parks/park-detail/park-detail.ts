import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { TooltipModule } from 'primeng/tooltip';
import { ParksService } from '../../../core/services/parks.service';
import { ParkDto, FieldDto } from '../../../core/models/park.models';

@Component({
  selector: 'app-park-detail',
  standalone: true,
  imports: [RouterLink, ButtonModule, SkeletonModule, TooltipModule],
  templateUrl: './park-detail.html',
  styleUrl: './park-detail.scss',
})
export class ParkDetailComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly parksService = inject(ParksService);

  readonly park = signal<ParkDto | null>(null);
  readonly fields = signal<FieldDto[]>([]);
  readonly loading = signal<boolean>(true);
  readonly error = signal<string | null>(null);

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

  sportIcon(sport: string): string {
    switch (sport) {
      case 'Football':   return 'pi pi-circle-fill';
      case 'Tennis':     return 'pi pi-circle-fill';
      case 'Basketball': return 'pi pi-circle-fill';
      default:           return 'pi pi-circle';
    }
  }
}
