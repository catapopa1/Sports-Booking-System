import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FieldDto } from '../models/park.models';

@Injectable({ providedIn: 'root' })
export class FieldsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/Fields`;

  getById(id: number): Promise<FieldDto> {
    return firstValueFrom(this.http.get<FieldDto>(`${this.base}/${id}`));
  }
}
