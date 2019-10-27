using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to indicate the result of updating a specific helpdesk
    /// </summary>
    public class UpdateHelpdeskResponse : BaseResponse
    {
        public bool result { get; set; }
    }
}
