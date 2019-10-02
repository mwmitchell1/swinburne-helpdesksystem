import { HttpClient } from "@angular/common/http";
import { ValidateNicknameRequest } from "src/app/data/requests/student/validate-nickname-request";
import { ValidateNicknameResponse } from "src/app/data/responses/nickname/validate-nickname-response";
import { Injectable } from "@angular/core";

@Injectable({providedIn: 'root'})
export class NicknameService {
    constructor(private client: HttpClient) {
        
    }

    validateNickname(request: ValidateNicknameRequest) {
        return this.client.post<ValidateNicknameResponse>("/api/student/validate", request);
    }
}