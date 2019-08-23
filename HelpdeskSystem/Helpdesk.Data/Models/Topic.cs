using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class Topic
    {
        public Topic()
        {
            Queueitem = new HashSet<Queueitem>();
        }

        public int TopicId { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Unit Unit { get; set; }
        public virtual ICollection<Queueitem> Queueitem { get; set; }
    }
}
