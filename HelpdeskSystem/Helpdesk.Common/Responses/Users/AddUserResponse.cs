using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Users
{
    /// <summary>
    /// Used to indicate the result of adding a new user
    /// </summary>
    public class AddUserResponse : BaseResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
