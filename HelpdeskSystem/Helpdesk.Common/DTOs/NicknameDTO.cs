using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.DTOs
{
    /// <summary>
    /// Used as a transfer object to return to the database
    /// </summary>
    public class NicknameDTO
    {
        public int ID { get; set; }

        public string Nickname { get; set; }

        public string StudentID { get; set; }
    }
}
