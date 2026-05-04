import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-setup-wizard-step-1',
  standalone: true,
  imports: [InputTextModule, FormsModule, ButtonModule],
  templateUrl: './setup-wizard-step-1.html',
  styleUrl: './setup-wizard-step-1.scss',
})
export class SetupWizardStep1 {
  homeAssistantUrl: string | undefined;
  homeAssistantToken: string | undefined;
  connectionVerified = false;

  testConnection() {
    // Implement connection testing logic here

    // fake
    this.connectionVerified = true;
  }
}
