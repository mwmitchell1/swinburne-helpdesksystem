using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Requests.Students
{
    /// <summary>
    /// This request is used to validate a student's nickname
    /// </summary>
    public class ValidateNicknameRequest : BaseRequest
    {
        public string Name { get; set; }

        public string SID { get; set; }
    }
}
