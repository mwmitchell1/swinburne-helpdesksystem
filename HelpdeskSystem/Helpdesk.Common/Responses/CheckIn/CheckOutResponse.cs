using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.CheckIn
{
    /// <summary>
    /// Used to indicate the result of checking out
    /// </summary>
    public class CheckOutResponse : BaseResponse
    {
        public bool Result { get; set; }
    }
}
