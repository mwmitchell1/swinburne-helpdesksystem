﻿using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to return the specified timespan by SpanId, and a status message
    /// </summary>
    public class GetTimeSpanResponse : BaseResponse
    {
        public TimeSpanDTO Timespan { get; set; }
    }
}
