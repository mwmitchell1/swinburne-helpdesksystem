using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.DTOs
{
    /// <summary>
    /// Data transfer object used to represent the helpdesk
    /// </summary>
    public class HelpdeskDTO
    {
        public int HelpdeskID { get; set; }

        public string Name { get; set; }

        public bool HasQueue { get; set; }

        public bool HasCheckIn { get; set; }

        public bool IsDisabled { get; set; }
    }
}
