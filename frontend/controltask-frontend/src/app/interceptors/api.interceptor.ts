import { Injectable, inject } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../environments/environment';

@Injectable()
export class ApiInterceptor implements HttpInterceptor {
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Clonar la solicitud y agregar la URL base
    const apiReq = request.clone({
      url: `${environment.apiUrl}/${request.url}`
    });

    return next.handle(apiReq).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Ha ocurrido un error.';
        
        if (error.error instanceof ErrorEvent) {
          errorMessage = `Error: ${error.error.message}`;
        } else {
          switch (error.status) {
            case 400:
              errorMessage = error.error?.message || 'Solicitud incorrecta.';
              break;
            case 401:
              errorMessage = 'No autorizado.';
              this.router.navigate(['/']);
              break;
            case 404:
              errorMessage = 'Recurso no encontrado.';
              break;
            case 500:
              errorMessage = 'Error interno del servidor.';
              break;
            default:
              errorMessage = `Error ${error.status}: ${error.message}`;
          }
        }

        this.snackBar.open(errorMessage, 'Cerrar', {
          duration: 5000,
          panelClass: ['error-snackbar']
        });

        return throwError(() => error);
      })
    );
  }
}