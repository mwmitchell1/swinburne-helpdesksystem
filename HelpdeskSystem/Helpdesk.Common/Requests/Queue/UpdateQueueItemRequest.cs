using System;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Common.Requests.Queue
{
    /// <summary>
    /// This request is used to update a queue item
    /// </summary>
    public class UpdateQueueItemRequest : BaseRequest
    {
        public int TopicID { get; set; }

        public string Description { get; set; }
    }
}
