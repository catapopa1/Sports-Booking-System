import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';
import { environment } from '../../../environments/environment';
import { NotificationDto } from '../models/notification.models';
import { PagedResult } from '../models/pagination.models';

@Injectable({ providedIn: 'root' })
export class NotificationsService {
  private readonly http = inject(HttpClient);
  private readonly base = `${environment.apiBaseUrl}/api/Notifications`;
  private readonly hubUrl = `${environment.apiBaseUrl}/hubs/notifications`;

  private connection: HubConnection | null = null;

  private readonly _unreadCount = signal(0);
  readonly unreadCount = this._unreadCount.asReadonly();

  /** Last notification pushed from SignalR. Pages subscribe via `effect()` to react to live arrivals. */
  private readonly _lastReceived = signal<NotificationDto | null>(null);
  readonly lastReceived = this._lastReceived.asReadonly();

  getPage(page: number, pageSize: number): Promise<PagedResult<NotificationDto>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return firstValueFrom(
      this.http.get<PagedResult<NotificationDto>>(this.base, { params })
    );
  }

  markRead(id: number): Promise<void> {
    return firstValueFrom(this.http.put<void>(`${this.base}/${id}/read`, null));
  }

  markAllRead(): Promise<void> {
    return firstValueFrom(this.http.put<void>(`${this.base}/read-all`, null));
  }

  async refreshUnreadCount(): Promise<void> {
    try {
      // Cap at 100 unread for the badge. Beyond that the user already sees noise on /notifications.
      const result = await this.getPage(1, 100);
      this._unreadCount.set(result.items.filter(n => !n.isRead).length);
    } catch {
      this._unreadCount.set(0);
    }
  }

  decrementUnread(by = 1): void {
    this._unreadCount.update(n => Math.max(0, n - by));
  }

  clearUnread(): void {
    this._unreadCount.set(0);
  }

  async connect(token: string): Promise<void> {
    if (this.connection && this.connection.state !== HubConnectionState.Disconnected) {
      return;
    }

    this.connection = new HubConnectionBuilder()
      .withUrl(this.hubUrl, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.connection.on('ReceiveNotification', (payload: NotificationDto) => {
      this._lastReceived.set(payload);
      if (!payload.isRead) {
        this._unreadCount.update(n => n + 1);
      }
    });

    try {
      await this.connection.start();
    } catch {
      this.connection = null;
    }
  }

  async disconnect(): Promise<void> {
    if (!this.connection) return;
    try {
      await this.connection.stop();
    } finally {
      this.connection = null;
      this._lastReceived.set(null);
    }
  }
}
