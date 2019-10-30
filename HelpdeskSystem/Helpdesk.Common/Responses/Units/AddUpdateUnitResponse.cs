using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Units
{
    /// <summary>
    /// Used to indicate the result of adding or updating a unit
    /// </summary>
    public class AddUpdateUnitResponse : BaseResponse
    {
        public int UnitID { get; set; }
    }
}
