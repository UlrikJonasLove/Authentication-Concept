import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { guestGuard } from './core/guards/guest.guard';
import { GuestLayoutShellComponent } from './layout/shell/guest-layout-shell.component';
import { MainLayoutShellComponent } from './layout/shell/main-layout-shell.component';

export const routes: Routes = [
  {
    path: 'login',
    component: GuestLayoutShellComponent,
    canActivate: [guestGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/auth/pages/login-page.component').then((module) => module.LoginPageComponent),
      },
    ],
  },
  {
    path: 'register',
    component: GuestLayoutShellComponent,
    canActivate: [guestGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/auth/pages/register-page.component').then(
            (module) => module.RegisterPageComponent,
          ),
      },
    ],
  },
  {
    path: 'dashboard',
    component: MainLayoutShellComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        loadComponent: () =>
          import('./features/dashboard/pages/dashboard-page.component').then(
            (module) => module.DashboardPageComponent,
          ),
      },
    ],
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
  {
    path: '**',
    redirectTo: 'login',
  },
];
