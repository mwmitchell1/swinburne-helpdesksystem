using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class Queueitem
    {
        public Queueitem()
        {
            Checkinqueueitem = new HashSet<Checkinqueueitem>();
        }

        public int ItemId { get; set; }
        public int StudentId { get; set; }
        public int TopicId { get; set; }
        public string NickName { get; set; }
        public DateTime TimeAdded { get; set; }
        public byte? TimeHelped { get; set; }
        public byte? TimeRemoved { get; set; }

        public virtual Nicknames Student { get; set; }
        public virtual Topic Topic { get; set; }
        public virtual ICollection<Checkinqueueitem> Checkinqueueitem { get; set; }
    }
}
