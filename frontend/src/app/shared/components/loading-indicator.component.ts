import { Component, input } from '@angular/core';

@Component({
  selector: 'app-loading-indicator',
  styleUrl: './loading-indicator.component.scss',
  templateUrl: './loading-indicator.component.html',
})
export class LoadingIndicatorComponent {
  readonly label = input<string | null>(null);
}
