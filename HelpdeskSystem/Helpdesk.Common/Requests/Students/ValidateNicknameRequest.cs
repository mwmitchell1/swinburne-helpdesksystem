using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Requests.Students
{
    public class ValidateNicknameRequest : BaseRequest
    {
        public string Name { get; set; }

        public string SID { get; set; }
    }
}
