using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;

namespace Helpdesk.Common.Responses.Units
{
    /// <summary>
    /// Used to return all units under a specific helpdesk and a status message
    /// </summary>
    public class GetUnitsByHelpdeskIDResponse : BaseResponse
    {
        public List<UnitDTO> Units { get; set; }
    }
}
