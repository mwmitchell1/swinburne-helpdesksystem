import { Topic } from "../../DTOs/topic.dto";

export class AddUpdateUnitRequest {
    public HelpdeskID: number;
    public Name: string;
    public IsDeleted: boolean;
    public Code: string;
    public Topics: string[];

    constructor() {
        this.Topics = [];
    }
}