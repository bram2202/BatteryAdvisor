import { Component, Input } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { EntityDto } from '../../../../models/entity-dto';


@Component({
  selector: 'app-setup-wizard-step-3',
  standalone: true,
  imports: [ButtonModule],
  templateUrl: './setup-wizard-step-3.html',
  styleUrl: './setup-wizard-step-3.scss',
})
export class SetupWizardStep3 {
  @Input() homeAssistantUrl: string = '';
  @Input() selectedPowerConsumptionEntities: EntityDto[] = [];
  @Input() selectedPowerProductionEntities: EntityDto[] = [];
  @Input() selectedPvEntities: EntityDto[] = [];

  save(): void {
    // Implementation pending
  }
}
