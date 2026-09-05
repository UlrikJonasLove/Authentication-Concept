import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { AbstractControl, ValidationErrors } from '@angular/forms';

@Component({
  selector: 'app-validation-message',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: './validation-message.component.scss',
  templateUrl: './validation-message.component.html',
})
export class ValidationMessageComponent {
  readonly control = input.required<AbstractControl>();
  readonly messages = input<Record<string, string>>({});

  protected readonly shouldShowMessage = (): boolean => {
    const control = this.control();

    return control.invalid && (control.touched || control.dirty);
  };

  protected readonly message = (): string | null => {
    const control = this.control();
    const errors = control.errors;

    if (!errors) {
      return null;
    }

    const [firstErrorKey] = Object.keys(errors);

    if (!firstErrorKey) {
      return null;
    }

    return this.resolveMessage(firstErrorKey, errors) ?? 'Please review this field.';
  };

  private readonly resolveMessage = (
    errorKey: string,
    errors: ValidationErrors,
  ): string | undefined => {
    const customMessage = this.messages()[errorKey];

    if (customMessage) {
      return customMessage;
    }

    if (errorKey === 'minlength') {
      const requiredLength = errors['minlength']?.requiredLength;

      return requiredLength ? `Use at least ${requiredLength} characters.` : undefined;
    }

    if (errorKey === 'maxlength') {
      const requiredLength = errors['maxlength']?.requiredLength;

      return requiredLength ? `Use no more than ${requiredLength} characters.` : undefined;
    }

    return undefined;
  };
}
