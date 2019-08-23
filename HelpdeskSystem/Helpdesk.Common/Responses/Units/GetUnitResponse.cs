using System;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Units
{
    public class GetUnitResponse : BaseResponse
    {
        public UnitDTO Unit { get; set; }
    }
}
