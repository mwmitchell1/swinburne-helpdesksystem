import { BaseResponse } from "../base-response";
import { Helpdesk } from "../../DTOs/helpdesk.dto";

export class GetHelpdesksResponse extends BaseResponse {
    public helpdesks: Helpdesk[];
}