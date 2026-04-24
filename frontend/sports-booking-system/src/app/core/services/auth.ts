import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { LoginRequest, RegisterRequest, LoginResult } from '../models/auth.models';


@Injectable({
  providedIn: 'root',
})

export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly router = inject(Router);
    private readonly base = `${environment.apiBaseUrl}/api/Auth`

    private readonly _user = signal<LoginResult | null>(this.loadFromStorage());

    readonly user = this._user.asReadonly();
    readonly isLoggedIn = computed(() => this._user() !== null);
    readonly role = computed(() => this._user()?.role ?? null);
    readonly token = computed(() => this._user()?.token ?? null);
    readonly userId = computed(() => this._user()?.userId ?? null);


    async login(request: LoginRequest): Promise<void> {
      const result = await firstValueFrom(
        this.http.post<LoginResult>(`${this.base}/login`,request)
      );

      localStorage.setItem('auth_user',JSON.stringify(result));
      this._user.set(result);
    }

    async register(request: RegisterRequest): Promise<{ id: number}> {
      return firstValueFrom(
        this.http.post<{ id:number }>(`${this.base}/register`,request)
      );
    }

    logout(): void {
      localStorage.removeItem('auth_user');
      this._user.set(null);
      this.router.navigate(['/login']);
    }
    

    private loadFromStorage(): LoginResult | null {
      const raw = localStorage.getItem('auth_user');
      if (!raw) 
        return null;

      try {
          return JSON.parse(raw) as LoginResult;
      } catch {
        return null;
      }
    }
}
