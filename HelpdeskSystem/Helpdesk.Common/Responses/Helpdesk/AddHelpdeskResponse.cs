using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to indicate the result of adding a new helpdesk
    /// </summary>
    public class AddHelpdeskResponse : BaseResponse
    {
        public int HelpdeskID { get; set; }
    }
}
