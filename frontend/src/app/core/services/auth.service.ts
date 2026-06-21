import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth.model';
import { User } from '../models/user.model';

const tokenKey = 'bookcatalog.token';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly tokenSignal = signal<string | null>(localStorage.getItem(tokenKey));

  readonly token = this.tokenSignal.asReadonly();
  readonly isLoggedIn = computed(() => !!this.tokenSignal());
  readonly currentUser = computed<User | null>(() => {
    const token = this.tokenSignal();
    if (!token) {
      return null;
    }

    const payload = this.decodePayload(token);
    if (!payload) {
      return null;
    }

    return {
      id: String(payload.sub ?? ''),
      username: String(payload.username ?? ''),
      email: String(payload.email ?? ''),
      createdAt: ''
    };
  });

  register(request: RegisterRequest) {
    return this.http.post<User>(`${environment.apiUrl}/auth/register`, request);
  }

  login(request: LoginRequest) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, request).pipe(
      tap((response) => this.setToken(response.token))
    );
  }

  logout(): void {
    localStorage.removeItem(tokenKey);
    this.tokenSignal.set(null);
  }

  private setToken(token: string): void {
    localStorage.setItem(tokenKey, token);
    this.tokenSignal.set(token);
  }

  private decodePayload(token: string): Record<string, unknown> | null {
    try {
      const payload = token.split('.')[1];
      if (!payload) {
        return null;
      }
      return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/'))) as Record<string, unknown>;
    } catch {
      return null;
    }
  }
}
