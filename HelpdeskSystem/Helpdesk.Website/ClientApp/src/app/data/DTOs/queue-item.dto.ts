export class QueueItem {
    public ItemId: number;
    public CheckInId?: number;
    public Nickname: string;
    public Topic: string;
    public Unit: string;
    public TimeAdded: Date;
    public TimeHelped?: Date;
    public TimeRemoved?: Date;
}