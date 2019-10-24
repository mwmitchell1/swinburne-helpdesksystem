import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
/**
 * Used to check that the user is logged in so that they check use the section
 */
export class AuthGuardService implements CanActivate {
    constructor(private cookieService: CookieService, private router: Router) { }

    /**
     * Function that checks that the user has an auth cookie in and redirects them to the log in page if they don't
     * @param route the source route that the request is coming from
     * @param state the route information
     */
    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.cookieService.get('AuthToken') !== '') {
            return true;
        } else {
            this.router.navigate(['/login'], {
                queryParams: {
                    return: state.url
                }
            });
            return false;
        }
    }
}
