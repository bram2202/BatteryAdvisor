import { inject, Injectable, signal } from '@angular/core';
import { ApiConfigurationService } from '../api-services/api-configuration-service/api-configuration-service';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationStatusService {
  private readonly apiConfigurationService = inject(ApiConfigurationService);

  readonly haConnectionConfigured = signal(false);
  readonly powerEntitiesConfigured = signal(false);

  async refresh(): Promise<void> {
    const [urlConfig, tokenConfig, entitiesConfig] = await Promise.all([
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantUrl'),
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantToken'),
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantPowerConsumptionEntities'),
    ]);

    this.haConnectionConfigured.set(urlConfig !== null && tokenConfig !== null);
    this.powerEntitiesConfigured.set(entitiesConfig !== null);
  }
}
