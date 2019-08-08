using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Common.Requests.Users
{
    /// <summary>
    /// Contains the user's new information, username must be no more than
    /// 20 characters
    /// </summary>
    public class UpdateUserRequest : BaseRequest
    {
        [StringLength(20, ErrorMessage = "Username cannot excede 20 characters")]
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
