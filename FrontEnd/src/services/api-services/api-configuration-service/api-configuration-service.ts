import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';

export interface ConfigurationReadModel {
  name: string;
  value: string;
}

@Injectable({
  providedIn: 'root',
})
export class ApiConfigurationService {
  private readonly httpClient = inject(HttpClient);

  async getConfigurationByKey(key: string): Promise<ConfigurationReadModel | null> {
    try {
      return await lastValueFrom(
        this.httpClient.get<ConfigurationReadModel>('configuration/by-key', { params: { key } }),
      );
    } catch {
      return null;
    }
  }

  async saveConfiguration(key: string, value: string): Promise<void> {
    const existing = await this.getConfigurationByKey(key);
    if (existing === null) {
      await lastValueFrom(this.httpClient.post('configuration', { name: key, value }));
    } else {
      await lastValueFrom(this.httpClient.put('configuration', { name: key, value }));
    }
  }
}
