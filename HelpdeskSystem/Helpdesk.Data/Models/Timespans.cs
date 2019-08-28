using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class Timespans
    {
        public int SpanId { get; set; }
        public int HelpdeskId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual Helpdesksettings Helpdesksettings { get; set; }
    }
}
