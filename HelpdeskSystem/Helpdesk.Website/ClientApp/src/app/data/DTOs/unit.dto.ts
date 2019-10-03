import { Topic } from "./topic.dto";

export class Unit {
    public unitId: number;
    public code: string;
    public name: string;
    public isDeleted: boolean;
    public topics: Topic[];
}