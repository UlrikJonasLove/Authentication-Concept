import { Component, effect, inject } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AppBrandComponent } from '../components/app-brand.component';

@Component({
  selector: 'app-guest-layout-shell',
  imports: [AppBrandComponent, RouterOutlet],
  styleUrl: './guest-layout-shell.component.scss',
  templateUrl: './guest-layout-shell.component.html',
})
export class GuestLayoutShellComponent {
  protected readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  constructor() {
    effect(() => {
      if (this.authService.isAuthenticated()) {
        void this.router.navigate(['/dashboard'], { replaceUrl: true });
      }
    });
  }
}
