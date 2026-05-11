import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MenubarModule } from 'primeng/menubar';
import { MenuModule } from 'primeng/menu';
import { MenuItem } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { ApiConfigurationService } from '../services/api-services/api-configuration-service/api-configuration-service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MenubarModule, MenuModule, ToastModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  protected readonly appName = 'Battery Advisor';

  private readonly apiConfigurationService = inject(ApiConfigurationService);

  protected readonly haConnectionConfigured = signal(false);
  protected readonly powerEntitiesConfigured = signal(false);

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
      badge: this.haConnectionConfigured() ? '✓' : undefined,
      badgeStyleClass: 'config-badge-success',
    },
    {
      label: 'Power Entity Setup',
      icon: 'fa-solid fa-bolt',
      routerLink: '/power-entity-setup',
      badge: this.powerEntitiesConfigured() ? '✓' : undefined,
      badgeStyleClass: 'config-badge-success',
      disabled: !this.haConnectionConfigured(),
    },
  ]);

  ngOnInit(): void {
    this.checkConfigurationStatus();
  }

  private async checkConfigurationStatus(): Promise<void> {
    const [urlConfig, tokenConfig, entitiesConfig] = await Promise.all([
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantUrl'),
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantToken'),
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantPowerConsumptionEntities'),
    ]);

    this.haConnectionConfigured.set(urlConfig !== null && tokenConfig !== null);
    this.powerEntitiesConfigured.set(entitiesConfig !== null);
  }
}
