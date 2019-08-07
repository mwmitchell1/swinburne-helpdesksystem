using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
