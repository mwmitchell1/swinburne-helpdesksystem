using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class Helpdeskunit
    {
        public int HelpdeskUnitId { get; set; }
        public int HelpdeskId { get; set; }
        public int UnitId { get; set; }

        public virtual Helpdesksettings Helpdesk { get; set; }
        public virtual Unit Unit { get; set; }
    }
}
