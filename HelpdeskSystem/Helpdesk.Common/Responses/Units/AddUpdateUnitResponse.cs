using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Units
{
    public class AddUpdateUnitResponse : BaseResponse
    {
        public int UnitID { get; set; }
        public int HelpDeskID { get; set; }
    }
}
