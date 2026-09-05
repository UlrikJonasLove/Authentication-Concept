import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { AppBrandComponent } from './app-brand.component';

@Component({
  selector: 'app-main-header',
  imports: [AppBrandComponent, RouterLink, RouterLinkActive],
  styleUrl: './main-header.component.scss',
  templateUrl: './main-header.component.html',
})
export class MainHeaderComponent {
  private readonly authService = inject(AuthService);

  protected readonly isLoggingOut = signal(false);
  protected readonly username = computed(() => this.authService.currentUser()?.username ?? 'Member');
  protected readonly initials = computed(() => this.username().slice(0, 1).toUpperCase());

  protected readonly logout = (): void => {
    if (this.isLoggingOut()) {
      return;
    }

    this.isLoggingOut.set(true);

    this.authService
      .logout()
      .pipe(finalize(() => this.isLoggingOut.set(false)))
      .subscribe();
  };
}
