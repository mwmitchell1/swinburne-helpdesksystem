using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to return the list of all timespans in the database, and a status message
    /// </summary>
    public class GetTimeSpansResponse : BaseResponse
    {
        public List<TimeSpanDTO> Timespans { get; set; }
    }
}
