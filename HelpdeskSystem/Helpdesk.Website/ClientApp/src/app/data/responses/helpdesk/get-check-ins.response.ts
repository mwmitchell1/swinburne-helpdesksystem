import { BaseResponse } from "../base-response";
import { CheckIn } from "../../DTOs/check-in.dto";

export class GetCheckInsResponse extends BaseResponse {
    public checkIns: CheckIn[];
}