import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MainHeaderComponent } from '../components/main-header.component';

@Component({
  selector: 'app-main-layout-shell',
  imports: [MainHeaderComponent, RouterOutlet],
  styleUrl: './main-layout-shell.component.scss',
  templateUrl: './main-layout-shell.component.html',
})
export class MainLayoutShellComponent {}
