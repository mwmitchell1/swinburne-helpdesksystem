using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Users
{
    /// <summary>
    /// Used to return the specified user by UserId, and a status message
    /// </summary>
    public class GetUserResponse : BaseResponse
    {
        public UserDTO User { get; set; }
    }
}
