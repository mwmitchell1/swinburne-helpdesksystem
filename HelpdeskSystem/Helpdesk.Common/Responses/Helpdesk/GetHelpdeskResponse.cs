using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    public class GetHelpdeskResponse : BaseResponse
    {
        public HelpdeskDTO Helpdesk { get; set; }
    }
}
