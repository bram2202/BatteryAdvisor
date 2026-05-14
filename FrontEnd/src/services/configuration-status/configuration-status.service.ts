import { inject, Injectable, signal } from '@angular/core';
import { ApiConfigurationService } from '../api-services/api-configuration-service/api-configuration-service';
import { ConfigurationKeyEnum } from '../../enums/configuration-key-enum';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationStatusService {
  private readonly apiConfigurationService = inject(ApiConfigurationService);

  readonly haConnectionConfigured = signal(false);
  readonly powerEntitiesConfigured = signal(false);
  readonly powerImportCostsConfigured = signal(false);
  readonly powerExportCostsConfigured = signal(false);

  async refresh(): Promise<void> {
    const configs = await this.apiConfigurationService.getConfigurations();
    const configMap = new Set(configs.map((c) => c.name));

    this.haConnectionConfigured.set(configMap.has(ConfigurationKeyEnum.HomeAssistantUrl));
    this.powerEntitiesConfigured.set(configMap.has(ConfigurationKeyEnum.HomeAssistantPowerConsumptionEntities));
    this.powerImportCostsConfigured.set(configMap.has(ConfigurationKeyEnum.PowerCostImport));
    this.powerExportCostsConfigured.set(configMap.has(ConfigurationKeyEnum.PowerCostExport));
  }
}
