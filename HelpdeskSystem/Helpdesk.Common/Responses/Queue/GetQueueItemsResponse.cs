﻿using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Queue
{
    /// <summary>
    /// Used to return the list of all queue items in the database, and a status message
    /// </summary>
    public class GetQueueItemsResponse : BaseResponse
    {
        public List<QueueItemDTO> QueueItems { get; set; }
    }
}
