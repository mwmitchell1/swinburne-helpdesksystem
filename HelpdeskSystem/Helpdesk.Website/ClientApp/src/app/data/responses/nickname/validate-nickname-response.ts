import { BaseResponse } from "../base-response";

export class ValidateNicknameResponse extends BaseResponse {
    public sid: number;
    public studentId: string;
    public nickname: string;
}