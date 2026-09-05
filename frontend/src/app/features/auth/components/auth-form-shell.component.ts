import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-auth-form-shell',
  imports: [RouterLink],
  styleUrl: './auth-form-shell.component.scss',
  templateUrl: './auth-form-shell.component.html',
})
export class AuthFormShellComponent {
  readonly eyebrow = input('Secure account access');
  readonly title = input.required<string>();
  readonly subtitle = input.required<string>();
  readonly footerPrompt = input.required<string>();
  readonly footerLinkLabel = input.required<string>();
  readonly footerLink = input.required<string>();
}
