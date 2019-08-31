import { BaseResponse } from "./base-response";

/**
 * This is used to represent that response returned by the api when a user attempts to login
 */
export class LoginResponse extends BaseResponse {
    public Token: string;
    public userId: string;
}