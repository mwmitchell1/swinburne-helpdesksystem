using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.CheckIn
{
    /// <summary>
    /// Used to indicate the result of checking in
    /// </summary>
    public class CheckInResponse : BaseResponse
    {
        public int CheckInID { get; set; }

        public int StudentID { get; set; }
    }
}
