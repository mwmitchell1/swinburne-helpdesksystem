using System;
namespace Helpdesk.Common.Responses.Queue
{
    /// <summary>
    /// Used to indicate the result of updating a queue item
    /// </summary>
    public class UpdateQueueItemResponse : BaseResponse
    {
        public bool Result { get; set; }
    }
}
