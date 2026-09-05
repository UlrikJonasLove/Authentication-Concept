import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { AuthFormShellComponent } from '../components/auth-form-shell.component';
import { LoadingIndicatorComponent } from '../../../shared/components/loading-indicator.component';
import { ValidationMessageComponent } from '../../../shared/components/validation-message.component';
import { AutofocusDirective } from '../../../shared/directives/autofocus.directive';

const usernameMessages = {
  required: 'Enter your username.',
  minlength: 'Use at least 3 characters.',
};

const passwordMessages = {
  required: 'Enter your password.',
  minlength: 'Use at least 8 characters.',
};

const getLoginErrorMessage = (error: unknown): string => {
  if (!(error instanceof HttpErrorResponse)) {
    return 'We could not sign you in right now.';
  }

  if (error.status === 0 || error.status >= 500) {
    return 'The authentication service is unavailable right now. Please try again shortly.';
  }

  return 'We could not sign you in with those credentials.';
};

@Component({
  selector: 'app-login-page',
  imports: [
    AuthFormShellComponent,
    AutofocusDirective,
    LoadingIndicatorComponent,
    ReactiveFormsModule,
    ValidationMessageComponent,
  ],
  styleUrl: './login-page.component.scss',
  templateUrl: './login-page.component.html',
})
export class LoginPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly submitError = signal<string | null>(null);
  protected readonly isSubmitting = signal(false);

  protected readonly usernameMessages = usernameMessages;
  protected readonly passwordMessages = passwordMessages;

  protected readonly loginForm = this.formBuilder.nonNullable.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  protected readonly submit = (): void => {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.submitError.set(null);
    this.isSubmitting.set(true);

    this.authService
      .login(this.loginForm.getRawValue())
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.authService.clearAuthNotice();
          void this.router.navigate(['/dashboard']);
        },
        error: (error: unknown) => {
          this.submitError.set(getLoginErrorMessage(error));
        },
      });
  };
}
