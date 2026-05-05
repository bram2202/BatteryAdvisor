import { inject, Injectable } from '@angular/core';
import { MessageService } from 'primeng/api';
import { PopupTypeEnum } from '../../enums/popup-type-enum';

@Injectable({
  providedIn: 'root',
})
export class PopupService {
  private readonly messageService = inject(MessageService);

  /**
    * Show a toast message with the given type, title and message
   * @param type Severity of the toast message
   * @param title Title of the toast message
   * @param message Detailed message of the toast
   */
  public showToast(type: PopupTypeEnum, title: string, message: string) {
    this.messageService.add({ severity: type, summary: title, detail: message });
  }
}
