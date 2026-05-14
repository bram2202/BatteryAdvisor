import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { ApiConfigurationService } from '../../../services/api-services/api-configuration-service/api-configuration-service';
import { ConfigurationKeyEnum } from '../../../enums/configuration-key-enum';
import { PopupService } from '../../../services/popup-service/popup-service';
import { PopupTypeEnum } from '../../../enums/popup-type-enum';
import { ConfigurationStatusService } from '../../../services/configuration-status/configuration-status.service';

@Component({
  selector: 'app-power-cost-setup',
  standalone: true,
  imports: [InputNumberModule, FormsModule, ButtonModule],
  templateUrl: './power-cost-setup.html',
  styleUrl: './power-cost-setup.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PowerCostSetup implements OnInit {

  private readonly apiConfigurationService = inject(ApiConfigurationService);
  private readonly popupService = inject(PopupService);
  private readonly configStatus = inject(ConfigurationStatusService);
  private readonly cdr = inject(ChangeDetectorRef);

  costImport: number = 0;
  costExport: number = 0;

  ngOnInit(): void {
    this.loadSavedValues();
  }

  private async loadSavedValues(): Promise<void> {
    const [importConfig, exportConfig] = await Promise.all([
      this.apiConfigurationService.getConfigurationByKey(ConfigurationKeyEnum.PowerCostImport),
      this.apiConfigurationService.getConfigurationByKey(ConfigurationKeyEnum.PowerCostExport),
    ]);

    if (importConfig?.value) {
      this.costImport = parseFloat(importConfig.value);
    }
    if (exportConfig?.value) {
      this.costExport = parseFloat(exportConfig.value);
    }
    this.cdr.markForCheck();
  }

  save(): void {
    Promise.all([
      this.apiConfigurationService.saveConfiguration(ConfigurationKeyEnum.PowerCostImport, this.costImport.toString()),
      this.apiConfigurationService.saveConfiguration(ConfigurationKeyEnum.PowerCostExport, this.costExport.toString()),
    ])
      .then(() => {
        this.configStatus.powerImportCostsConfigured.set(true);
        this.configStatus.powerExportCostsConfigured.set(true);
        this.popupService.showToast(
          PopupTypeEnum.Success,
          'Configuration Saved',
          'Your cost configuration has been saved successfully.',
        );
      })
      .catch((error: unknown) => {
        console.error('Failed to save cost configuration:', error);
        this.popupService.showToast(
          PopupTypeEnum.Error,
          'Save Failed',
          'An error occurred while saving your configuration. Please try again.',
        );
      });
  }
}
