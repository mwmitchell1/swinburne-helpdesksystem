import { HttpClient } from "@angular/common/http";
import { ValidateNicknameRequest } from "src/app/data/requests/student/validate-nickname-request";
import { ValidateNicknameResponse } from "src/app/data/responses/nickname/validate-nickname-response";
import { Injectable } from "@angular/core";
import { GetAllNicknamesResponses } from "src/app/data/responses/nickname/get-all-nicknames-response";
import { GenerateNicknameResponse } from "src/app/data/responses/nickname/generate-nickname-response";
import { EditStudentNicknameResponse } from "src/app/data/responses/nickname/edit-nickname-response";
import { EditStudentNicknameRequest } from "src/app/data/requests/student/edit-student-nickname-request";

@Injectable({providedIn: 'root'})
export class NicknameService {
    constructor(private client: HttpClient) {
        
    }

    validateNickname(request: ValidateNicknameRequest) {
        return this.client.post<ValidateNicknameResponse>("/api/student/validate", request);
    }

    getNickames() {
        return this.client.get<GetAllNicknamesResponses>("/api/student/");
    }

    generateNickname() {
        return this.client.get<GenerateNicknameResponse>("/api/student/generate");
    }

    editNickname(id: number, request: EditStudentNicknameRequest) {
        return this.client.patch<EditStudentNicknameResponse>('/api/student/nickname/' + id, request);
    }
}