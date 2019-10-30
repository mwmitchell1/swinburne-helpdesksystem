using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to indicate the result of getting all helpdesks
    /// </summary>
    public class GetHelpdesksResponse : BaseResponse
    {
        public List<HelpdeskDTO> Helpdesks { get; set; }

        public GetHelpdesksResponse()
        {
            Helpdesks = new List<HelpdeskDTO>();
        }
    }
}
