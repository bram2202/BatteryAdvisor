import { Component, inject, signal } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { PopupService } from '../../../services/popup-service/popup-service';
import { PopupTypeEnum } from '../../../enums/popup-type-enum';
import { ApiTestService } from '../../../services/api-services/api-test-service/api-test-service';
import { FormsModule } from '@angular/forms';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-home-assistant-connection',
  standalone: true,
  imports: [ButtonModule, FormsModule, InputTextModule],
  templateUrl: './home-assistant-connection.html',
  styleUrl: './home-assistant-connection.scss',
})
export class HomeAssistantConnection {
  private readonly apiTestService = inject(ApiTestService);
  private readonly popupService = inject(PopupService);

  readonly homeAssistantUrl = signal('http://homeassistant.local:8123');
  readonly homeAssistantToken = signal<string | undefined>(undefined);

  get canSave(): boolean {
    return !!this.homeAssistantUrl() && !!this.homeAssistantToken();
  }

  testAndSaveConnection() {
    this.apiTestService
      .testApiConnection(this.homeAssistantUrl(), this.homeAssistantToken() ?? '')
      .then(() => {
        this.popupService.showToast(
          PopupTypeEnum.Success,
          'Connection Successful',
          'Successfully connected to Home Assistant',
        );
      })
      .catch((error) => {
        console.error('Connection failed:', error);
        this.popupService.showToast(
          PopupTypeEnum.Error,
          'Connection Failed',
          'Failed to connect to Home Assistant',
        );
      });
  }
}
