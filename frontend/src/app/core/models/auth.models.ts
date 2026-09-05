export interface AuthUser {
  userId: string;
  username: string;
}

export interface AuthResponse extends AuthUser {
  accessToken: string;
  accessTokenExpiresAt: string;
}

export interface LoginRequest {
  username: string;
  password: string;
}

export interface RegisterRequest extends LoginRequest {}

export type AuthStatus = 'checking' | 'authenticated' | 'anonymous';

export interface AuthState {
  status: AuthStatus;
  user: AuthUser | null;
  accessToken: string | null;
  accessTokenExpiresAt: string | null;
  authNotice: string | null;
}
