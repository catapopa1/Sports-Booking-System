import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SportType, UpsertSportProfileRequest, UserProfileDto, UserSearchResultDto } from '../models/user.models';
import { PagedResult } from '../models/pagination.models';

@Injectable({ providedIn: 'root' })
export class UsersService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/Users`;

  getMyProfile(): Promise<UserProfileDto> {
    return firstValueFrom(this.http.get<UserProfileDto>(`${this.base}/me`));
  }

  getById(id: number): Promise<UserProfileDto> {
    return firstValueFrom(this.http.get<UserProfileDto>(`${this.base}/${id}`));
  }

  search(query: string, page = 1, pageSize = 20): Promise<PagedResult<UserSearchResultDto>> {
    const params = new HttpParams()
      .set('q', query)
      .set('page', page)
      .set('pageSize', pageSize);
    return firstValueFrom(
      this.http.get<PagedResult<UserSearchResultDto>>(`${this.base}/search`, { params })
    );
  }

  updateBio(bio: string | null): Promise<void> {
    return firstValueFrom(this.http.put<void>(`${this.base}/me`, { bio }));
  }

  changePassword(currentPassword: string, newPassword: string): Promise<void> {
    return firstValueFrom(
      this.http.put<void>(`${this.base}/me/password`, { currentPassword, newPassword })
    );
  }

  uploadAvatar(file: File): Promise<{ url: string }> {
    const formData = new FormData();
    formData.append('file', file);
    return firstValueFrom(
      this.http.post<{ url: string }>(`${this.base}/me/avatar`, formData)
    );
  }

  upsertSportProfile(sport: SportType, body: UpsertSportProfileRequest): Promise<void> {
    return firstValueFrom(
      this.http.put<void>(`${this.base}/me/sports/${sport}`, body)
    );
  }
}
