import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MenubarModule } from 'primeng/menubar';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MenubarModule, MenuModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly appName = 'Battery Advisor';

  protected readonly topMenuItems: MenuItem[] = [
    {
      label: 'Home',
      routerLink: '/',
    },
    {
      label: 'Setup Wizard',
      routerLink: '/setup-wizard',
    },
  ];

  protected readonly sideMenuItems: MenuItem[] = [
    {
      label: 'Home',
      icon: 'fa-solid fa-house',
      routerLink: '/',
    },
    {
      label: 'Setup Wizard',
      icon: 'fa-solid fa-gears',
      routerLink: '/setup-wizard',
    },
  ];
}
