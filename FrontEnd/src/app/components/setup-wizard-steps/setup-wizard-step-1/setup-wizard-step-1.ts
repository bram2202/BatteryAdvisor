import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ApiTestService } from '../../../../services/api-services/api-test-service/api-test-service';
import { PopupService } from '../../../../services/popup-service/popup-service';
import { PopupTypeEnum } from '../../../../enums/popup-type-enum';

@Component({
  selector: 'app-setup-wizard-step-1',
  standalone: true,
  imports: [InputTextModule, FormsModule, ButtonModule],
  templateUrl: './setup-wizard-step-1.html',
  styleUrl: './setup-wizard-step-1.scss',
})
export class SetupWizardStep1 {
  homeAssistantUrl: string = 'http://homeassistant.local:8123';
  homeAssistantToken: string | undefined;
  connectionVerified = false;

  private readonly apiTestService = inject(ApiTestService);
  private readonly popupService = inject(PopupService);

  testConnection() {
    this.apiTestService
      .testApiConnection(this.homeAssistantUrl, this.homeAssistantToken!)
      .then(() => {
        this.connectionVerified = true;
        this.popupService.showToast(
          PopupTypeEnum.Success,
          'Connection Successful',
          'Successfully connected to Home Assistant',
        );
      })
      .catch((error) => {
        this.connectionVerified = false
        console.error('Connection failed:', error);
        this.popupService.showToast(
          PopupTypeEnum.Error,
          'Connection Failed',
          'Failed to connect to Home Assistant',
        );
      });
  }
}
