using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.DTOs
{
    public class CheckInDTO
    {
        public int CheckInId { get; set; }
        public string Nickname { get; set; }
        public int UnitId { get; set; }
        public int StudentId { get; set; }
    }
}
