using Helpdesk.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Queue
{
    /// <summary>
    /// Used to return the list of all queue items in the database, and a status message
    /// </summary>
    public class GetQueueItemsByHelpdeskIDResponse : BaseResponse
    {
        public List<QueueItemDTO> QueueItems { get; set; }
    }
}
