import { Component, computed, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MenubarModule } from 'primeng/menubar';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ConfigurationStatusService } from '../services/configuration-status/configuration-status.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MenubarModule, MenuModule, ToastModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  protected readonly appName = 'Battery Advisor';

  protected readonly configStatus = inject(ConfigurationStatusService);

  protected readonly sideMenuItems = computed<MenuItem[]>(() => [
    {
      label: 'Home',
      icon: 'fa-solid fa-house',
      routerLink: '/',
    },
    {
      label: 'HA Connection',
      icon: 'fa-solid fa-plug',
      routerLink: '/home-assistant-connection',
      badge: this.configStatus.haConnectionConfigured() ? '✓' : undefined,
      badgeStyleClass: 'config-badge-success',
    },
    {
      label: 'Power Entity Setup',
      icon: 'fa-solid fa-bolt',
      routerLink: '/power-entity-setup',
      badge: this.configStatus.powerEntitiesConfigured() ? '✓' : undefined,
      badgeStyleClass: 'config-badge-success',
      disabled: !this.configStatus.haConnectionConfigured(),
    },
    {
      label: 'Power Cost Setup',
      icon: 'fa-solid fa-dollar-sign',
      routerLink: '/power-cost-setup',
      badge: this.configStatus.powerImportCostsConfigured() && this.configStatus.powerExportCostsConfigured() ? '✓' : undefined,
      badgeStyleClass: 'config-badge-success',
      disabled: !this.configStatus.powerEntitiesConfigured(),
    },
  ]);

  ngOnInit(): void {
    this.configStatus.refresh();
  }
}
