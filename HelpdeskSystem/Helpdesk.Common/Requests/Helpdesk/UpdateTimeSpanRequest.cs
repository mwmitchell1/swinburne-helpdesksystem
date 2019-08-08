using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Requests.Helpdesk
{
    /// <summary>
    /// Contains the timespan's new information
    /// </summary>
    public class UpdateTimeSpanRequest : BaseRequest
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
