using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Users
{
    /// <summary>
    /// Contains the user's new information, username must be no more than
    /// 20 characters
    /// </summary>
    public class UpdateUserRequest : BaseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Username cannot be empty!")]
        [StringLength(20, ErrorMessage = "Username cannot excede 20 characters")]
        public string Username { get; set; }

        [MinLength(6)]
        public string Password { get; set; }
    }
}
