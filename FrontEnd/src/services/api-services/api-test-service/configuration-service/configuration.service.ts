import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ConfigurationService {
  private readonly env: Record<string, unknown> =
    (globalThis as typeof globalThis & { __env?: Record<string, unknown> }).__env || {};

  /**
   * Generic function to get a configuration value with type safety
   * @param key Configuration key
   * @param returnType Type
   */
  public getConfig<T = string>(key: string): T {
    const value = this.env[key];
    return value as T;
  }
}
