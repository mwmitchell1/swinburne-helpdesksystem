using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Students
{
    /// <summary>
    /// Used to indicate the result of getting all nicknames
    /// </summary>
    public class GetAllNicknamesResponse : BaseResponse
    {
        public List<NicknameDTO> Nicknames { get; set; }

        public GetAllNicknamesResponse()
        {
            Nicknames = new List<NicknameDTO>();
        }
    }
}
