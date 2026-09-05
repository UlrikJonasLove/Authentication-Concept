import { HttpErrorResponse, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { API_BASE_URL } from '../../app.tokens';
import { AuthService } from '../services/auth.service';

const authPaths = ['/auth/login', '/auth/register', '/auth/logout', '/auth/renew'];

const isApiRequest = (request: HttpRequest<unknown>, apiBaseUrl: string): boolean =>
  request.url.startsWith(apiBaseUrl);

const isAuthRequest = (request: HttpRequest<unknown>, apiBaseUrl: string): boolean =>
  authPaths.some((path) => request.url.startsWith(`${apiBaseUrl}${path}`));

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authService = inject(AuthService);
  const apiBaseUrl = inject(API_BASE_URL);

  const shouldAttachToken = isApiRequest(request, apiBaseUrl) && !isAuthRequest(request, apiBaseUrl);
  const accessToken = authService.accessToken();
  const attemptedAuthenticatedRequest = shouldAttachToken && Boolean(accessToken);

  const authenticatedRequest = attemptedAuthenticatedRequest
    ? request.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
    : request;

  return next(authenticatedRequest).pipe(
    catchError((error: unknown) => {
      if (
        !(error instanceof HttpErrorResponse) ||
        error.status !== 401 ||
        !attemptedAuthenticatedRequest
      ) {
        return throwError(() => error);
      }

      return authService.renewAccessToken().pipe(
        switchMap((renewedSession) =>
          next(
            request.clone({
              setHeaders: {
                Authorization: `Bearer ${renewedSession.accessToken}`,
              },
            }),
          ),
        ),
        catchError((renewError: unknown) => throwError(() => renewError)),
      );
    }),
  );
};
