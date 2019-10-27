using System;
namespace Helpdesk.Common.Responses.Helpdesk
{
    /// <summary>
    /// Used to indicate the result of forcing a checkout of a queue item
    /// </summary>
    public class ForceCheckoutQueueRemoveResponse: BaseResponse
    {
        public bool Result { get; set; }
    }
}
