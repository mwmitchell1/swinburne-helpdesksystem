using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Queue
{
    /// <summary>
    /// Used to indicate the result of adding an item to a queue
    /// </summary>
    public class AddToQueueResponse : BaseResponse
    {
        public int ItemId { get; set; }
    }
}
