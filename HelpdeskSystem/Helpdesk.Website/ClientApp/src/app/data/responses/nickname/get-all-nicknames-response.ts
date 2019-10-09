import { BaseResponse } from "../base-response";
import { Nickname } from "../../DTOs/nickname.dto";

export class GetAllNicknamesResponses extends BaseResponse {
    public nicknames: Nickname[];
}