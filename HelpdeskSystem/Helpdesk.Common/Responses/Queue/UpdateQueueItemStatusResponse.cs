using System;
namespace Helpdesk.Common.Responses.Queue
{
    /// <summary>
    /// Used to indicate the result of updating queue item status.
    /// </summary>
    public class UpdateQueueItemStatusResponse : BaseResponse
    {
        public bool Result { get; set; }
    }
}
