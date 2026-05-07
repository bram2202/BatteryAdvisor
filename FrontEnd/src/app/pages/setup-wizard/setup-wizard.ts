import { Component, inject, signal } from '@angular/core';
import { StepperModule } from 'primeng/stepper';
import { ButtonModule } from 'primeng/button';
import { SetupWizardStep1 } from '../../components/setup-wizard-steps/setup-wizard-step-1/setup-wizard-step-1';
import { SetupWizardStep2 } from '../../components/setup-wizard-steps/setup-wizard-step-2/setup-wizard-step-2';
import { SetupWizardStep3 } from '../../components/setup-wizard-steps/setup-wizard-step-3/setup-wizard-step-3';
import { EntityDto } from '../../../models/entity-dto';
import { ApiEntityService } from '../../../services/api-services/api-entity-service/api-entity-service';
import { PopupService } from '../../../services/popup-service/popup-service';
import { PopupTypeEnum } from '../../../enums/popup-type-enum';

@Component({
  selector: 'app-setup-wizard',
  standalone: true,
  imports: [StepperModule, ButtonModule, SetupWizardStep1, SetupWizardStep2, SetupWizardStep3],
  templateUrl: './setup-wizard.html',
  styleUrl: './setup-wizard.scss',
})
export class SetupWizard {
  private readonly apiEntityService = inject(ApiEntityService);
  private readonly popupService = inject(PopupService);

  homeAssistantUrl = signal('http://homeassistant.local:8123');
  homeAssistantToken = signal<string | undefined>(undefined);
  selectedPowerConsumptionEntities = signal<EntityDto[]>([]);
  selectedPowerProductionEntities = signal<EntityDto[]>([]);
  selectedPvEntities = signal<EntityDto[]>([]);

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
          'Your configuration has been saved successfully.',
        );

        // Redirect to ......
      })
      .catch((error) => {
        console.error('Failed to save statistic entities:', error);
        this.popupService.showToast(
          PopupTypeEnum.Error,
          'Save Failed',
          'An error occurred while saving your configuration. Please try again.',
        );
      });
  }
}
