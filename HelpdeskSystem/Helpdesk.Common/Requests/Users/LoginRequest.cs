using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Users
{
    /// <summary>
    /// Used to transfer the information required for a user to login
    /// </summary>
    public class LoginRequest : BaseRequest
    {
        [Required (AllowEmptyStrings = false, ErrorMessage = "Please enter a username.")]
        public string Username { get; set; }

        [Required (AllowEmptyStrings = false, ErrorMessage = "Please enter a password.")]
        public string Password { get; set; }
    }
}
