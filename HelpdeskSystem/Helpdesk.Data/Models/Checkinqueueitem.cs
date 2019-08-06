using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class Checkinqueueitem
    {
        public int Id { get; set; }
        public int QueueItemId { get; set; }
        public int CheckInId { get; set; }

        public virtual Checkinhistory CheckIn { get; set; }
        public virtual Queueitem QueueItem { get; set; }
    }
}
