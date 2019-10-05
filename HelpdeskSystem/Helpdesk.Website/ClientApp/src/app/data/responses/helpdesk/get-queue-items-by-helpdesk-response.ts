import { BaseResponse } from "../base-response";
import { QueueItem } from "../../DTOs/queue-item.dto";

export class  GetQueueItemsByHelpdeskIDResponse extends BaseResponse {
    public queueItems: QueueItem[];
}