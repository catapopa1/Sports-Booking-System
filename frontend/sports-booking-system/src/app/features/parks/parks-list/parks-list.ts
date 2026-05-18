import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { SelectModule } from 'primeng/select';
import { SkeletonModule } from 'primeng/skeleton';
import { ButtonModule } from 'primeng/button';
import { ParksService } from '../../../core/services/parks.service';
import { ParkSummaryDto } from '../../../core/models/park.models';

@Component({
  selector: 'app-parks-list',
  standalone: true,
  imports: [
    FormsModule,
    RouterLink,
    InputTextModule,
    SelectModule,
    SkeletonModule,
    ButtonModule,
  ],
  templateUrl: './parks-list.html',
  styleUrl: './parks-list.scss',
})
export class ParksListComponent {
  private readonly parksService = inject(ParksService);

  readonly parks = signal<ParkSummaryDto[]>([]);
  readonly loading = signal<boolean>(true);
  readonly error = signal<string | null>(null);
  readonly search = signal<string>('');
  readonly cityFilter = signal<string | null>(null);

  readonly cityOptions = computed(() => {
    const unique = [...new Set(this.parks().map(p => p.city))].sort();
    return [{ label: 'All cities', value: null }, ...unique.map(c => ({ label: c, value: c }))];
  });

  readonly filteredParks = computed(() => {
    const q = this.search().trim().toLowerCase();
    const city = this.cityFilter();
    return this.parks().filter(p => {
      const matchesQuery = !q || p.name.toLowerCase().includes(q) || p.city.toLowerCase().includes(q);
      const matchesCity = !city || p.city === city;
      return matchesQuery && matchesCity;
    });
  });

  constructor() {
    this.loadParks();
  }

  async loadParks(): Promise<void> {
    this.loading.set(true);
    this.error.set(null);
    try {
      const data = await this.parksService.getAll();
      this.parks.set(data);
    } catch {
      this.error.set('Failed to load parks. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }
}
