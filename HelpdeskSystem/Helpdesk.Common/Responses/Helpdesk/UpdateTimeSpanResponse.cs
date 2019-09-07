using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to return a bool representing whether the timespan has been successfully
    /// updated or not, and a status message
    /// </summary>
    public class UpdateTimeSpanResponse : BaseResponse
    {
        public bool Result { get; set; }
    }
}
