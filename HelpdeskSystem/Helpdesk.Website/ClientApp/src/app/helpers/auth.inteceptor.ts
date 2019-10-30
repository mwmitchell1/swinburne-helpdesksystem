import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
/**
 * This is used to add the auth token to every request from the Angular Web App
 */
export class AuthInterceptor implements HttpInterceptor {

    constructor(private cookieService: CookieService) {
    }

    /**
     * This function receives a request adds the bearer token to it
     * @param req the request that requires the token
     * @param next the client that is processing the request
     */
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        req = req.clone({
            setHeaders: {
                'Authorization': 'Bearer ' + this.cookieService.get('AuthToken'),
            }
        });

        return next.handle(req);
    }
}
