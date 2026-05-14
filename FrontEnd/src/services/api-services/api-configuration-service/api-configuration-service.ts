import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { ConfigurationKeyEnum } from '../../../enums/configuration-key-enum';

export interface ConfigurationReadModel {
  name: string;
  value: string;
}

@Injectable({
  providedIn: 'root',
})
export class ApiConfigurationService {
  private readonly httpClient = inject(HttpClient);

  async getConfigurations(): Promise<ConfigurationReadModel[]> {
    return await lastValueFrom(this.httpClient.get<ConfigurationReadModel[]>('configuration'));
  }

  async getConfigurationByKey(key: ConfigurationKeyEnum): Promise<ConfigurationReadModel | null> {
    try {
      return await lastValueFrom(
        this.httpClient.get<ConfigurationReadModel>('configuration/by-key', { params: { key } }),
      );
    } catch {
      return null;
    }
  }

  async saveConfiguration(key: ConfigurationKeyEnum, value: string): Promise<void> {
      await lastValueFrom(this.httpClient.post('configuration', { name: key, value }));
  }
}
