using System;
using System.Collections.Generic;
using System.Text;
<<<<<<< .merge_file_lWnUFB

namespace Helpdesk.Common.Responses.Helpdesk
{
    public class GetTimeSpanResponse : BaseResponse
    {
=======
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to return the specified timespan by SpanId, and a status message
    /// </summary>
    public class GetTimeSpanResponse : BaseResponse
    {
        public TimeSpanDTO Timespan { get; set; }
>>>>>>> .merge_file_E50xlU
    }
}
