using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Users
{
    public class LoginRequest : BaseRequest
    {
        [Required (AllowEmptyStrings = false, ErrorMessage = "Please enter a username.")]
        public string Username { get; set; }

        [Required (AllowEmptyStrings = false, ErrorMessage = "Please enter a password.")]
        public string Password { get; set; }
    }
}
