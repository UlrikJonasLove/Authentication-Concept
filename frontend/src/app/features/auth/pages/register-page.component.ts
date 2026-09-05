import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';
import { LoadingIndicatorComponent } from '../../../shared/components/loading-indicator.component';
import { ValidationMessageComponent } from '../../../shared/components/validation-message.component';
import { AutofocusDirective } from '../../../shared/directives/autofocus.directive';
import { AuthFormShellComponent } from '../components/auth-form-shell.component';

const usernameMessages = {
  required: 'Choose a username.',
  minlength: 'Use at least 3 characters.',
};

const passwordMessages = {
  required: 'Create a password.',
  minlength: 'Use at least 8 characters.',
};

const confirmPasswordMessages = {
  required: 'Confirm your password.',
};

const passwordsMatchValidator = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;

  if (!password || !confirmPassword || password === confirmPassword) {
    return null;
  }

  return { passwordsMismatch: true };
};

const getRegisterErrorMessage = (error: unknown): string => {
  if (!(error instanceof HttpErrorResponse)) {
    return 'We could not create your account right now.';
  }

  if (error.status === 0 || error.status >= 500) {
    return 'The authentication service is unavailable right now. Please try again shortly.';
  }

  if (error.status === 409) {
    return 'That username is already in use. Try another one.';
  }

  return 'We could not complete registration with those details.';
};

@Component({
  selector: 'app-register-page',
  imports: [
    AuthFormShellComponent,
    AutofocusDirective,
    LoadingIndicatorComponent,
    ReactiveFormsModule,
    ValidationMessageComponent,
  ],
  styleUrl: './register-page.component.scss',
  templateUrl: './register-page.component.html',
})
export class RegisterPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly submitError = signal<string | null>(null);
  protected readonly isSubmitting = signal(false);

  protected readonly usernameMessages = usernameMessages;
  protected readonly passwordMessages = passwordMessages;
  protected readonly confirmPasswordMessages = confirmPasswordMessages;

  protected readonly registerForm = this.formBuilder.nonNullable.group(
    {
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: [passwordsMatchValidator] },
  );

  protected readonly submit = (): void => {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }

    const { username, password } = this.registerForm.getRawValue();

    this.submitError.set(null);
    this.isSubmitting.set(true);

    this.authService
      .register({ username, password })
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.authService.clearAuthNotice();
          void this.router.navigate(['/dashboard']);
        },
        error: (error: unknown) => {
          this.submitError.set(getRegisterErrorMessage(error));
        },
      });
  };
}
