import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AdminUserDto, UserRole } from '../models/user.models';
import { PagedResult } from '../models/pagination.models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/Admin`;

  getUsers(query: string, page: number, pageSize: number): Promise<PagedResult<AdminUserDto>> {
    const params = new HttpParams()
      .set('q', query)
      .set('page', page)
      .set('pageSize', pageSize);
    return firstValueFrom(
      this.http.get<PagedResult<AdminUserDto>>(`${this.base}/users`, { params })
    );
  }

  updateRole(userId: number, role: UserRole): Promise<void> {
    return firstValueFrom(
      this.http.put<void>(
        `${this.base}/users/${userId}/role`,
        JSON.stringify(role),
        { headers: { 'Content-Type': 'application/json' } }
      )
    );
  }
}
