using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class Helpdesksettings
    {
        public Helpdesksettings()
        {
            Helpdeskunit = new HashSet<Helpdeskunit>();
        }

        public int HelpdeskId { get; set; }
        public string Name { get; set; }
        public bool HasCheckIn { get; set; }
        public bool HasQueue { get; set; }
        public bool IsDeleted { get; set; }

        public virtual ICollection<Timespans> Timespans { get; set; }
        public virtual ICollection<Helpdeskunit> Helpdeskunit { get; set; }
    }
}
