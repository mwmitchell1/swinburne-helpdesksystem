export class QueueItem {
    public itemId: number;
    public checkInId?: number;
    public nickname: string;
    public topic: string;
    public description: string;
    public unit: string;
    public TimeAdded: Date;
    public TimeHelped?: Date;
    public TimeRemoved?: Date;
}
