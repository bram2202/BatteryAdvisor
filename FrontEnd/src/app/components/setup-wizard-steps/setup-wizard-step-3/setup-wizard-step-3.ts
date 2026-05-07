import { Component, Input, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { EntityDto } from '../../../../models/entity-dto';
import { WritableSignal } from '@angular/core';

@Component({
  selector: 'app-setup-wizard-step-3',
  standalone: true,
  imports: [ButtonModule],
  templateUrl: './setup-wizard-step-3.html',
  styleUrl: './setup-wizard-step-3.scss',
})
export class SetupWizardStep3 {
  @Input() homeAssistantUrlSignal!: WritableSignal<string>;
  @Input() selectedPowerConsumptionEntitiesSignal!: WritableSignal<EntityDto[]>;
  @Input() selectedPowerProductionEntitiesSignal!: WritableSignal<EntityDto[]>;
  @Input() selectedPvEntitiesSignal!: WritableSignal<EntityDto[]>;
}
