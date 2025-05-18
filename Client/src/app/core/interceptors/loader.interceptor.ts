import { HttpInterceptorFn } from '@angular/common/http';
import { LoadingService } from '../services/loading.service';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';

export const loaderInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(LoadingService);
  busyService.busy();
  return next(req).pipe(
    finalize(() => {
      busyService.idle()
    })
  );
};
