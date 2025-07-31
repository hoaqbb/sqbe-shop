import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import {
  BehaviorSubject,
  catchError,
  filter,
  switchMap,
  take,
  throwError,
} from 'rxjs';
import { AccountService } from '../services/account.service';
import { Router } from '@angular/router';

let isRefreshing = false;
// save state of token refresh
const refreshTokenSubject = new BehaviorSubject<boolean>(false);

export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const accountService = inject(AccountService);
  const router = inject(Router);

  return next(req.clone({ withCredentials: true })).pipe(
    catchError((error) => {
      if (error.status === 401) {
        if (!isRefreshing) {
          isRefreshing = true;
          refreshTokenSubject.next(false);

          return accountService.refreshToken().pipe(
            switchMap(() => {
              isRefreshing = false;
              refreshTokenSubject.next(true);
              // Retry the original request
              return next(req.clone({ withCredentials: true }));
            }),
            // Handle errors during token refresh
            catchError((refreshError) => {
              // Logout the user and redirect to login page
              isRefreshing = false;
              accountService.removeCurrentUserSource();
              // router.navigateByUrl('/login');
              return throwError(
                () => new Error('Session expired. Please login again.')
              );
            })
          );
        } else {
          // Wait until refresh completes
          return refreshTokenSubject.pipe(
            filter((refreshed) => refreshed === true),
            take(1),
            switchMap(() => next(req.clone({ withCredentials: true })))
          );
        }
      }
      return throwError(() => error);
    })
  );
};
