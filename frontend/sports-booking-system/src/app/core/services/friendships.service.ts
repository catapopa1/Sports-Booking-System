import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { FriendDto, FriendRequestDto, FriendshipStatus } from '../models/friendship.models';

@Injectable({ providedIn: 'root' })
export class FriendshipsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/Friendship`;

  sendRequest(addresseeId: number): Promise<{ id: number }> {
    return firstValueFrom(
      this.http.post<{ id: number }>(this.base, { addresseeId })
    );
  }

  getMyFriends(): Promise<FriendDto[]> {
    return firstValueFrom(this.http.get<FriendDto[]>(`${this.base}/me`));
  }

  getPending(): Promise<FriendRequestDto[]> {
    return firstValueFrom(this.http.get<FriendRequestDto[]>(`${this.base}/pending`));
  }

  getSent(): Promise<FriendRequestDto[]> {
    return firstValueFrom(this.http.get<FriendRequestDto[]>(`${this.base}/sent`));
  }

  respondToRequest(friendshipId: number, status: FriendshipStatus): Promise<void> {
    return firstValueFrom(
      this.http.put<void>(
        `${this.base}/${friendshipId}/respond`,
        JSON.stringify(status),
        { headers: { 'Content-Type': 'application/json' } }
      )
    );
  }

  removeFriend(friendshipId: number): Promise<void> {
    return firstValueFrom(this.http.delete<void>(`${this.base}/${friendshipId}`));
  }
}
