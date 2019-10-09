using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Students
{
    /// <summary>
    /// Response used to return a generated nickname
    /// </summary>
    public class GenerateNicknameResponse : BaseResponse
    {
        public string Nickname { get; set; }
    }
}
