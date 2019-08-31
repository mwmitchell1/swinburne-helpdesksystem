using System;
using System.Collections.Generic;

namespace Helpdesk.Data.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool FirstTime { get; set; }
    }
}
