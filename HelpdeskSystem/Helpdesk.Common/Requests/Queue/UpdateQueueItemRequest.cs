using System;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Common.Requests.Queue
{
    public class UpdateQueueItemRequest : BaseRequest
    {
        public int TopicID { get; set; }
    }
}
