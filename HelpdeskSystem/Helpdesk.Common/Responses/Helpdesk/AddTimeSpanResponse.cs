using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to indicate the result of adding a new timespan
    /// </summary>
    public class AddTimeSpanResponse : BaseResponse
    {
        public int SpanId { get; set; }
    }
}
