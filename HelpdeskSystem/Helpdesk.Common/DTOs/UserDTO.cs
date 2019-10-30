using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.DTOs
{
    /// <summary>
    /// Data transfer object used to represent a user
    /// </summary>
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool FirstTime { get; set; }
    }
}
