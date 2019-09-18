using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Requests.CheckIn
{
    /// <summary>
    /// This request is used to determine whether a check out item is forced or not
    /// </summary>
    public class CheckOutRequest
    {
        public bool? ForcedCheckout { get; set; }
    }
}
