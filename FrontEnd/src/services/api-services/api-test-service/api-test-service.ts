import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ApiTestService {
  private readonly httpClient = inject(HttpClient);

  /**
   * Test the API connection with the provided URL and token.
   * @param url Home Assistant URL to test against.
   * @param token Home Assistant access token to use for authentication.
   * @returns A promise that resolves with the test result from the backend.
   */
  async testApiConnection(url: string, token: string): Promise<any> {
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
