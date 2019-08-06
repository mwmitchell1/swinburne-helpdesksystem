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
        public byte? HasCheckIn { get; set; }
        public byte? HasQueue { get; set; }

        public virtual ICollection<Helpdeskunit> Helpdeskunit { get; set; }
    }
}
