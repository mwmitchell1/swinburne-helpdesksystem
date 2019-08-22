using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class Unit
    {
        public Unit()
        {
            Checkinhistory = new HashSet<Checkinhistory>();
            Helpdeskunit = new HashSet<Helpdeskunit>();
            Topic = new HashSet<Topic>();
        }

        public int UnitId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Checkinhistory> Checkinhistory { get; set; }
        public virtual ICollection<Helpdeskunit> Helpdeskunit { get; set; }
        public virtual ICollection<Topic> Topic { get; set; }
    }
}
