import { Unit } from "../../DTOs/unit.dto";
import { BaseResponse } from "../base-response";

export class GetUnitsByHelpdeskIdResponse extends BaseResponse {
    public units: Unit[];
}