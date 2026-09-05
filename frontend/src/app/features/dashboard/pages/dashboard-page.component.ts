import { DatePipe } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-dashboard-page',
  imports: [DatePipe],
  styleUrl: './dashboard-page.component.scss',
  templateUrl: './dashboard-page.component.html',
})
export class DashboardPageComponent {
  private readonly authService = inject(AuthService);

  protected readonly user = this.authService.currentUser;
  protected readonly accessTokenExpiresAt = this.authService.accessTokenExpiresAt;
  protected readonly sessionFacts = computed(() => [
    {
      title: 'In-memory access token',
      description: 'The access token only lives in Angular state and is never persisted to browser storage.',
    },
    {
      title: 'HttpOnly refresh cookie',
      description: 'The browser sends the refresh token cookie automatically on renew and logout requests.',
    },
    {
      title: 'Single renew in flight',
      description: 'Multiple expired requests wait for one refresh call before they retry.',
    },
  ]);
}
