import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const toastr = inject(ToastrService);

  return next(req).pipe(
    catchError(error => {
      if (error) {
        switch (error.status) {
          case 400:
            if (error.error.errors) {
              const modalStateErrors: any[] = [];
              for (const key in error.error.errors) {
                if (error.error.errors[key]) {
                  modalStateErrors.push(error.error.errors[key]);
                }
              }
              return throwError(() => modalStateErrors.flat());
            } else {
              toastr.error(error.error, error.status.toString());
              return throwError(() => error);
            }
          case 401:
            toastr.error('Unauthorized', error.status.toString());
            return throwError(() => error);
          case 404:
            router.navigateByUrl('/not-found');
            return throwError(() => error);
          case 500:
            const navigationExtras: NavigationExtras = { state: { error: error.error } };
            toastr.error('Server error', error.status.toString());
            return throwError(() => error);
          default:
            toastr.error('Something unexpected went wrong');
            console.log(error);
            return throwError(() => error);
        }
      }
      return throwError(() => error);
    })
  );
};  
