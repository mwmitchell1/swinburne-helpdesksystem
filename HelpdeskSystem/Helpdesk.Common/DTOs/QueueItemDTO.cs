using System;

namespace Helpdesk.Common.DTOs
{
    /// <summary>
    /// Data transfer object used to represent a queue item
    /// </summary>
    public class QueueItemDTO
    {
        public int ItemId { get; set; }
        public int? CheckInId { get; set; }
        public int StudentId { get; set; }
        public string Nickname { get; set; }
        public int TopicId { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public DateTime TimeAdded { get; set; }
        public DateTime? TimeHelped { get; set; }
        public DateTime? TimeRemoved { get; set; }
    }
}
