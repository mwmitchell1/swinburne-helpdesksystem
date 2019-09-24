using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class Nicknames
    {
        public Nicknames()
        {
            Queueitem = new HashSet<Queueitem>();
        }

        public int StudentId { get; set; }
        public string Sid { get; set; }
        public string NickName { get; set; }

        public virtual ICollection<Checkinhistory> Checkinhistory { get; set; }
        public virtual ICollection<Queueitem> Queueitem { get; set; }
    }
}
