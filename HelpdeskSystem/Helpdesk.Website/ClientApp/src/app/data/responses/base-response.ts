/**
 * Used as a base class for all response so that they all have a status code and relevent messages
 */
export class BaseResponse {
    public statusMessages: StatusMessage[];
    public status: number;
}

/**
 * Class used to represent the status messages
 */
export class StatusMessage {
    public MessageStatus: number;
    public Message: string;
}