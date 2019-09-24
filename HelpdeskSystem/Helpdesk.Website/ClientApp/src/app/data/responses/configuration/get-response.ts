import { BaseResponse } from "../base-response";
import { Helpdesk } from "../../DTOs/helpdesk.dto";

/**
 * This is used to represent that response returned by the api when retreiving a helpdesks configuration
 */
export class GetHelpdeskResponse extends BaseResponse {
    public helpdesk: Helpdesk;
}