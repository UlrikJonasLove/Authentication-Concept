import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-brand',
  imports: [RouterLink],
  styleUrl: './app-brand.component.scss',
  templateUrl: './app-brand.component.html',
})
export class AppBrandComponent {
  readonly compact = input(false);
}
