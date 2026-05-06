import { Component } from '@angular/core';
import { StepperModule } from 'primeng/stepper';
import { ButtonModule } from 'primeng/button';
import { SetupWizardStep1 } from '../../components/setup-wizard-steps/setup-wizard-step-1/setup-wizard-step-1';
import { SetupWizardStep2 } from '../../components/setup-wizard-steps/setup-wizard-step-2/setup-wizard-step-2';
import { SetupWizardStep3 } from '../../components/setup-wizard-steps/setup-wizard-step-3/setup-wizard-step-3';
import { EntityDto } from '../../../models/entity-dto';

@Component({
  selector: 'app-setup-wizard',
  standalone: true,
  imports: [StepperModule, ButtonModule, SetupWizardStep1, SetupWizardStep2, SetupWizardStep3],
  templateUrl: './setup-wizard.html',
  styleUrl: './setup-wizard.scss',
})
export class SetupWizard {
  homeAssistantUrl: string = '';
  selectedPowerConsumptionEntities: EntityDto[] = [];
  selectedPowerProductionEntities: EntityDto[] = [];
  selectedPvEntities: EntityDto[] = [];

  collectStep1(step: SetupWizardStep1): void {
    this.homeAssistantUrl = step.homeAssistantUrl;
  }

  collectStep2(step: SetupWizardStep2): void {
    this.selectedPowerConsumptionEntities = [...step.selectedPowerConsumptionEntities];
    this.selectedPowerProductionEntities = [...step.selectedPowerProductionEntities];
    this.selectedPvEntities = [...step.selectedPvEntities];
  }
}
