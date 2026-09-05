import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import {
  catchError,
  finalize,
  firstValueFrom,
  map,
  Observable,
  of,
  shareReplay,
  tap,
  throwError,
  timeout,
} from 'rxjs';
import { API_BASE_URL } from '../../app.tokens';
import { AuthResponse, AuthState, LoginRequest, RegisterRequest } from '../models/auth.models';

interface RenewOptions {
  redirectOnFailure?: boolean;
  unauthorizedNotice?: string | null;
  unavailableNotice?: string | null;
}

const initialAuthState: AuthState = {
  status: 'checking',
  user: null,
  accessToken: null,
  accessTokenExpiresAt: null,
  authNotice: null,
};

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly httpClient = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly apiBaseUrl = inject(API_BASE_URL);

  private readonly authStateSignal = signal<AuthState>(initialAuthState);
  private renewRequest$: Observable<AuthResponse> | null = null;

  readonly authState = this.authStateSignal.asReadonly();
  readonly status = computed(() => this.authStateSignal().status);
  readonly currentUser = computed(() => this.authStateSignal().user);
  readonly accessToken = computed(() => this.authStateSignal().accessToken);
  readonly accessTokenExpiresAt = computed(() => this.authStateSignal().accessTokenExpiresAt);
  readonly isAuthenticated = computed(() => this.authStateSignal().status === 'authenticated');
  readonly isRestoringSession = computed(() => this.authStateSignal().status === 'checking');
  readonly authNotice = computed(() => this.authStateSignal().authNotice);

  readonly login = (payload: LoginRequest): Observable<AuthResponse> =>
    this.httpClient
      .post<AuthResponse>(this.buildEndpoint('/auth/login'), payload, {
        withCredentials: true,
      })
      .pipe(tap((response) => this.setAuthenticatedState(response)));

  readonly register = (payload: RegisterRequest): Observable<AuthResponse> =>
    this.httpClient
      .post<AuthResponse>(this.buildEndpoint('/auth/register'), payload, {
        withCredentials: true,
      })
      .pipe(tap((response) => this.setAuthenticatedState(response)));

  readonly renewAccessToken = (options: RenewOptions = {}): Observable<AuthResponse> => {
    if (this.renewRequest$) {
      return this.renewRequest$;
    }

    const {
      redirectOnFailure = true,
      unauthorizedNotice = redirectOnFailure ? 'Your session ended. Sign in again to continue.' : null,
      unavailableNotice = 'The authentication service is temporarily unavailable.',
    } = options;

    this.renewRequest$ = this.httpClient
      .post<AuthResponse>(this.buildEndpoint('/auth/renew'), {}, { withCredentials: true })
      .pipe(
        timeout({ first: 8_000 }),
        tap((response) => this.setAuthenticatedState(response)),
        catchError((error: unknown) => {
          const httpError =
            error instanceof HttpErrorResponse ? error : new HttpErrorResponse({ error });

          if (httpError.status === 0 || httpError.status >= 500) {
            this.setAnonymousState(unavailableNotice);
          } else {
            this.setAnonymousState(unauthorizedNotice);
          }

          if (redirectOnFailure) {
            void this.router.navigate(['/login']);
          }

          return throwError(() => httpError);
        }),
        finalize(() => {
          this.renewRequest$ = null;
        }),
        shareReplay({ bufferSize: 1, refCount: false }),
      );

    return this.renewRequest$;
  };

  readonly restoreSession = async (): Promise<void> => {
    this.authStateSignal.update((state) => ({ ...state, status: 'checking' }));

    await firstValueFrom(
      this.renewAccessToken({
        redirectOnFailure: false,
        unauthorizedNotice: 'Your saved session could not be restored. Please sign in again.',
        unavailableNotice: 'The authentication service is temporarily unavailable.',
      }).pipe(
        map(() => void 0),
        catchError(() => of(void 0)),
      ),
    );
  };

  readonly logout = (): Observable<void> =>
    this.httpClient.post<void>(this.buildEndpoint('/auth/logout'), {}, { withCredentials: true }).pipe(
      catchError(() => of(void 0)),
      tap(() => this.setAnonymousState(null)),
      finalize(() => {
        void this.router.navigate(['/login']);
      }),
    );

  readonly clearAuthNotice = (): void => {
    this.authStateSignal.update((state) => ({ ...state, authNotice: null }));
  };

  private readonly setAuthenticatedState = (response: AuthResponse): void => {
    this.authStateSignal.set({
      status: 'authenticated',
      user: {
        userId: response.userId,
        username: response.username,
      },
      accessToken: response.accessToken,
      accessTokenExpiresAt: response.accessTokenExpiresAt,
      authNotice: null,
    });
  };

  private readonly setAnonymousState = (authNotice: string | null): void => {
    this.authStateSignal.set({
      status: 'anonymous',
      user: null,
      accessToken: null,
      accessTokenExpiresAt: null,
      authNotice,
    });
  };

  private readonly buildEndpoint = (path: string): string => `${this.apiBaseUrl}${path}`;
}
