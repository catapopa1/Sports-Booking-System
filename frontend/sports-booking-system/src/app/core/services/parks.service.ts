import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ParkSummaryDto, ParkDto, FieldDto, ParkStatsDto, ParkPhotoDto } from '../models/park.models';

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

  uploadPhoto(parkId: number, file: File): Promise<ParkPhotoDto> {
    const formData = new FormData();
    formData.append('file', file);
    return firstValueFrom(
      this.http.post<ParkPhotoDto>(`${this.base}/${parkId}/photos`, formData)
    );
  }

  deletePhoto(parkId: number, photoId: number): Promise<void> {
    return firstValueFrom(
      this.http.delete<void>(`${this.base}/${parkId}/photos/${photoId}`)
    );
  }

  setMainPhoto(parkId: number, photoId: number): Promise<void> {
    return firstValueFrom(
      this.http.put<void>(`${this.base}/${parkId}/photos/${photoId}/main`, {})
    );
  }
}
