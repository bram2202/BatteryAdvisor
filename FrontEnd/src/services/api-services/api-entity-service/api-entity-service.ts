import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { EntityDto } from '../../../models/entity-dto';
import { StatisticEntitiesDto } from '../../../models/statistic-entities-dto';

@Injectable({
  providedIn: 'root',
})
export class ApiEntityService {
  private readonly httpClient = inject(HttpClient);

  /**
   * Get list of metric entities from the backend.
   */
  async getEntities(): Promise<EntityDto[]> {
    return lastValueFrom(
      this.httpClient.get<EntityDto[]>('entity/entities')
    );
  }
  
  async saveStatisticEntity(statisticEntities: StatisticEntitiesDto): Promise<void> {
    await lastValueFrom(
      this.httpClient.post('entity/statistic-entities', statisticEntities)
    );
  }
}
