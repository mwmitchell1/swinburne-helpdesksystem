using System;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Units
{
    /// <summary>
    /// Used to indicate the result of getting a unit
    /// </summary>
    public class GetUnitResponse : BaseResponse
    {
        public UnitDTO Unit { get; set; }
    }
}
