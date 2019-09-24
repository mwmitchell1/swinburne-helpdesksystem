using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Common.Requests.Queue
{
    /// <summary>
    /// This request is used to update the status of a queue item.
    /// </summary>
    public class UpdateQueueItemStatusRequest : BaseRequest
    {
        public DateTime? TimeHelped { get; set; }
        public DateTime? TimeRemoved { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (TimeHelped == null && TimeRemoved == null)
            {
                results.Add(new ValidationResult("Both helped and removed dates are null."));
            }

            if (TimeHelped != null && TimeRemoved != null)
            {
                results.Add(new ValidationResult("Student cannot be helped and removed in a single request."));
            }

            return results;
        }
    }
}
