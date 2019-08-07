using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Users
{
    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }
    }
}
