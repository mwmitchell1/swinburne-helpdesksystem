using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.DTOs
{
    public class TimeSpanDTO
    {
        public int SpanId { get; set; }
        public int? HelpdeskId { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
