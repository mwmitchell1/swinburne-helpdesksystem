using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to indicate the result of getting a specifc helpdesk
    /// </summary>
    public class GetHelpdeskResponse : BaseResponse
    {
        public HelpdeskDTO Helpdesk { get; set; }
    }
}
