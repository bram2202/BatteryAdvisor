import { Component } from '@angular/core';
import { StepperModule } from 'primeng/stepper';
import { ButtonModule } from 'primeng/button';
import { SetupWizardStep1 } from '../../components/setup-wizard-steps/setup-wizard-step-1/setup-wizard-step-1';

@Component({
  selector: 'app-setup-wizard',
  standalone: true,
  imports: [StepperModule, ButtonModule, SetupWizardStep1],
  templateUrl: './setup-wizard.html',
  styleUrl: './setup-wizard.scss',
})
export class SetupWizard {}
