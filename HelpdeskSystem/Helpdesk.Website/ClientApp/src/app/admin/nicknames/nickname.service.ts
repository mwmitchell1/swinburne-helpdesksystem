import { HttpClient } from '@angular/common/http';
import { ValidateNicknameRequest } from 'src/app/data/requests/student/validate-nickname-request';
import { ValidateNicknameResponse } from 'src/app/data/responses/nickname/validate-nickname-response';
import { Injectable } from '@angular/core';
import { GetAllNicknamesResponses } from 'src/app/data/responses/nickname/get-all-nicknames-response';
import { GenerateNicknameResponse } from 'src/app/data/responses/nickname/generate-nickname-response';
import { EditStudentNicknameResponse } from 'src/app/data/responses/nickname/edit-nickname-response';
import { EditStudentNicknameRequest } from 'src/app/data/requests/student/edit-student-nickname-request';

@Injectable({providedIn: 'root'})
/**
 * this is used to call the CRUD APIs for students
 */
export class NicknameService {
    constructor(private client: HttpClient) {
    }

    /**
     * Used to call the API to validate and get nicknames for student entry
     * @param request The students information
     */
    validateNickname(request: ValidateNicknameRequest) {
        return this.client.post<ValidateNicknameResponse>('/api/student/validate', request);
    }

    /**
     * Used to call the API used to get all units
     */
    getNickames() {
        return this.client.get<GetAllNicknamesResponses>('/api/student/');
    }

    /**
     * Used to call the random nickname generator API
     */
    generateNickname() {
        return this.client.get<GenerateNicknameResponse>('/api/student/generate');
    }

    /**
     * used to call the API to update a nickname
     * @param id the system id of the student
     * @param request the students information
     */
    editNickname(id: number, request: EditStudentNicknameRequest) {
        return this.client.patch<EditStudentNicknameResponse>('/api/student/nickname/' + id, request);
    }
}
