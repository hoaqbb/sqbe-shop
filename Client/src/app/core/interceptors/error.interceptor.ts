import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastr = inject(ToastrService);
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if(error) {
        switch (error.status) {
          case 400: //Do nothing when accessToken is expired
            break;

          case 404:
            router.navigateByUrl('/not-found');
            break;

          case 409: //conflict
            toastr.error(error.error, error.status.toString());
            break;

          case 500:
            const navigationExtras: NavigationExtras = { state: {error: error.error}};
            router.navigateByUrl('/server-error', navigationExtras);
            break;
            
          default:
            console.log(error);
            break;
        }
      }
      return throwError(() => error);
    })
  );
};
