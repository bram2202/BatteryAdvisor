import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ApiTestService } from '../../../../services/api-services/api-test-service/api-test-service';

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

  private readonly apiTestService = inject(ApiTestService);

  testConnection() {
   
      // call ApiTestService.testApiConnection with homeAssistantUrl and homeAssistantToken
      // if successful, set connectionVerified to true
      // if failed, show an error message (for simplicity, we just log it here)
      console.log('Testing connection with URL:', this.homeAssistantUrl, 'and Token:', this.homeAssistantToken);

      this.apiTestService.testApiConnection(this.homeAssistantUrl!, this.homeAssistantToken!)
        .then(() => {
          console.log('Connection successful!');
          this.connectionVerified = true;
        })
        .catch((error) => {
          console.error('Connection failed:', error);
          alert('Failed to connect to Home Assistant');
        });

  }
}

// http://192.168.1.9:8123
// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJmNTU3NWY4ZTI1MmY0ZWIyYmM4NjZhYTY4OWVhNmQ0MyIsImlhdCI6MTc3NTgyNTE0MCwiZXhwIjoyMDkxMTg1MTQwfQ.Eg7uq7rfNdXg4nKG1ystaa823gqlybomGQ73cIlRpwM