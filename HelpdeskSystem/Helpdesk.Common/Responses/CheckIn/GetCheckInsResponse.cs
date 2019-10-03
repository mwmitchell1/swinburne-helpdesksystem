using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.CheckIn
{
    public class GetCheckInsResponse : BaseResponse
    {
        public List<CheckInDTO> CheckIns { get; set; }

        public GetCheckInsResponse()
        {
            CheckIns = new List<CheckInDTO>();
        }
    }
}
