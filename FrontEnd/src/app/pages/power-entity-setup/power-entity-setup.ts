import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { MultiSelectModule } from 'primeng/multiselect';
import { EntityDto } from '../../../models/entity-dto';
import { ApiEntityService } from '../../../services/api-services/api-entity-service/api-entity-service';
import { ApiConfigurationService } from '../../../services/api-services/api-configuration-service/api-configuration-service';
import { PopupService } from '../../../services/popup-service/popup-service';
import { PopupTypeEnum } from '../../../enums/popup-type-enum';

@Component({
  selector: 'app-power-entity-setup',
  standalone: true,
  imports: [ButtonModule, FormsModule, MultiSelectModule],
  templateUrl: './power-entity-setup.html',
  styleUrl: './power-entity-setup.scss',
})
export class PowerEntitySetup implements OnInit {
  private readonly apiEntityService = inject(ApiEntityService);
  private readonly apiConfigurationService = inject(ApiConfigurationService);
  private readonly popupService = inject(PopupService);

  readonly entities = signal<EntityDto[]>([]);
  readonly selectedPowerConsumptionEntities = signal<EntityDto[]>([]);
  readonly selectedPowerProductionEntities = signal<EntityDto[]>([]);
  readonly selectedPvEntities = signal<EntityDto[]>([]);

  get canProceed(): boolean {
    return (
      this.selectedPowerConsumptionEntities().length > 0 &&
      this.selectedPowerProductionEntities().length > 0
    );
  }

  ngOnInit(): void {
    this.loadSavedSelections();
  }

  private async loadSavedSelections(): Promise<void> {
    const [allEntities, consumptionConfig, productionConfig, pvConfig] = await Promise.all([
      this.apiEntityService.getEntities(),
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantPowerConsumptionEntities'),
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantPowerProductionEntities'),
      this.apiConfigurationService.getConfigurationByKey('HomeAssistantPvEntities'),
    ]);

    this.entities.set(allEntities);

    const toEntities = (config: { value: string } | null): EntityDto[] => {
      if (!config?.value) return [];
      const ids = new Set(config.value.split(',').map((id) => id.trim()).filter(Boolean));
      return allEntities.filter((e) => ids.has(e.entityId));
    };

    this.selectedPowerConsumptionEntities.set(toEntities(consumptionConfig));
    this.selectedPowerProductionEntities.set(toEntities(productionConfig));
    this.selectedPvEntities.set(toEntities(pvConfig));
  }

  onSelectionChange(type: 'consumption' | 'production' | 'pv', selected: EntityDto[]): void {
    if (type === 'consumption') {
      this.selectedPowerConsumptionEntities.set(selected);
    } else if (type === 'production') {
      this.selectedPowerProductionEntities.set(selected);
    } else {
      this.selectedPvEntities.set(selected);
    }
  }

  save(): void {
    const consumption = this.selectedPowerConsumptionEntities();
    const production = this.selectedPowerProductionEntities();
    const pv = this.selectedPvEntities();

    this.apiEntityService
      .saveStatisticEntity({
        pvEntities: pv.map((entity) => entity.entityId),
        powerProductionEntities: production.map((entity) => entity.entityId),
        powerConsumptionEntities: consumption.map((entity) => entity.entityId),
      })
      .then(() => {
        this.popupService.showToast(
          PopupTypeEnum.Success,
          'Configuration Saved',
          'Your entity configuration has been saved successfully.',
        );
      })
      .catch((error: unknown) => {
        console.error('Failed to save statistic entities:', error);
        this.popupService.showToast(
          PopupTypeEnum.Error,
          'Save Failed',
          'An error occurred while saving your configuration. Please try again.',
        );
      });
  }
}
