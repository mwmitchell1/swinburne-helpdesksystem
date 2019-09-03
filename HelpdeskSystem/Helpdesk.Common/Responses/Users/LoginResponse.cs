using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Users
{
    /// <summary>
    /// This is used to return the bearer token for the user if they have sucessfully logged in.
    /// It will return the relevent status messages if they have not
    /// </summary>
    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }

        public int? UserId { get; set; }
    }
}
