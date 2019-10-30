using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Students
{
    /// <summary>
    /// Used to indicate the result of validating a student's nickname
    /// </summary>
    public class ValidateNicknameResponse : BaseResponse
    {
        public int? SID { get; set; }
        public string Nickname { get; set; }
        public string StudentId { get; set; }
    }
}
