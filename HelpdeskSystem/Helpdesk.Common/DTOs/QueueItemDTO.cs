using System;

namespace Helpdesk.Common.DTOs
{
    public class QueueItemDTO
    {
        public int ItemId { get; set; }
        public int StudentId { get; set; }
        public int TopicId { get; set; }
        public DateTime TimeAdded { get; set; }
        public DateTime? TimeHelped { get; set; }
        public DateTime? TimeRemoved { get; set; }
    }
}
