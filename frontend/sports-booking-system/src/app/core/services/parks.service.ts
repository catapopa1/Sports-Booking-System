import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ParkSummaryDto, ParkDto, FieldDto, ParkStatsDto } from '../models/park.models';

@Injectable({ providedIn: 'root' })
export class ParksService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/Parks`;

  getAll(): Promise<ParkSummaryDto[]> {
    return firstValueFrom(this.http.get<ParkSummaryDto[]>(this.base));
  }

  getById(id: number): Promise<ParkDto> {
    return firstValueFrom(this.http.get<ParkDto>(`${this.base}/${id}`));
  }

  getFields(parkId: number): Promise<FieldDto[]> {
    return firstValueFrom(this.http.get<FieldDto[]>(`${this.base}/${parkId}/fields`));
  }

  getStats(parkId: number): Promise<ParkStatsDto> {
    return firstValueFrom(this.http.get<ParkStatsDto>(`${this.base}/${parkId}/stats`));
  }
}
