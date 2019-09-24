using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Students
{
    /// <summary>
    /// Used to return the result of searching for the student
    /// </summary>
    public class GetStudentResponse : BaseResponse
    {
        public NicknameDTO Nickname { get; set; }
    }
}
