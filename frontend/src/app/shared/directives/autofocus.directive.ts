import { afterNextRender, Directive, ElementRef, inject } from '@angular/core';

@Directive({
  selector: '[appAutofocus]',
})
export class AutofocusDirective {
  private readonly elementRef = inject<ElementRef<HTMLElement>>(ElementRef);

  constructor() {
    afterNextRender(() => {
      const element = this.elementRef.nativeElement;

      if ('focus' in element && !element.hasAttribute('disabled')) {
        element.focus();
      }
    });
  }
}
