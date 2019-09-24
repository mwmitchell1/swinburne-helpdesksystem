import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { LoginResponse } from '../data/responses/login-response';
import { LoginRequest } from '../data/requests/login-request';
import { CookieService } from 'ngx-cookie-service';

@Injectable({
    providedIn: 'root',
})
/**
 * This is used to call the authentication services to call the system api controllers for authentication
 */
export class AuthenticationService {
    private client: HttpClient;
    private baseUrl: string;

    constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private cookieService: CookieService) {
        this.client = http;
        this.baseUrl = baseUrl;
    }

    /**
     * Used to call the login api
     * @param loginRequest
     */
    loginUser(loginRequest) {
        return this.client.post<LoginResponse>(this.baseUrl + 'api/users/login', loginRequest)
    }

    /**
     * Used to call the logout api
     */
    logout() {
        return this.client.get(this.baseUrl + 'api/users/logout');
    }

    /**
     * Used to check if the user is logged in by checking if they have an auth token
     */
    isLoggedIn() {
        return this.cookieService.get('AuthToken') !== '';
    }

    verifyUser() {
        return this.client.get(this.baseUrl + 'api/users/verifyuser')
    }
}
