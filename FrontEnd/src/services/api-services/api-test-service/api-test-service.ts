import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiTestService {
  private readonly httpClient = inject(HttpClient);

  async testApiConnection(url: string, token: string) {
    return await lastValueFrom(
      this.httpClient.get('HomeAssistant/test-connection', {
        params: {
          url: url,
          token: token,
        },
      }),
    );
  }
}
