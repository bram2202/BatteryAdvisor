import { HttpEvent, HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ConfigurationService } from '../services/api-services/api-test-service/configuration-service/configuration.service';
import { inject } from '@angular/core';


/**
 * An HTTP interceptor that prepends the base URL to outgoing requests.
 */
export function BaseUrlInterceptor(
  req: HttpRequest<unknown>,
  next: HttpHandlerFn,
): Observable<HttpEvent<unknown>> {
  const configurationService = inject(ConfigurationService);

  // TODO: FIX
  //  const baseUrl = configurationService.getConfig<string>('API_URL');
  const baseUrl ='http://localhost:5056';
  console.log('BaseUrlInterceptor: Intercepting request to', req.url, 'with base URL', baseUrl);

  // Skip rewriting for asset requests
  const isAsset = req.url.startsWith('/i18n/') || req.url.startsWith('/assets/');

  if (isAsset) {
    return next(req);
  }

  const apiReq = req.clone({
    url: `${baseUrl}/${req.url}`,
  });
  return next(apiReq);
}