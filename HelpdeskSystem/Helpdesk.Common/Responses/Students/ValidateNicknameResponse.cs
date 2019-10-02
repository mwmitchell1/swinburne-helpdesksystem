using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Students
{
    public class ValidateNicknameResponse : BaseResponse
    {
        public int? SID { get; set; }
        public string Nickname { get; set; }
        public string StudentId { get; set; }
    }
}
