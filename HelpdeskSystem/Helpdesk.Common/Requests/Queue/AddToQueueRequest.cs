
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

/// <summary>
/// This request is used to add a queue item to the helpdesk
/// </summary>
namespace Helpdesk.Common.Requests.Queue
{
    public class AddToQueueRequest : BaseRequest
    {
        public int? StudentID { get; set; }

        public int TopicID { get; set; }

        [Required (AllowEmptyStrings = false, ErrorMessage = "You must enter in a description")]
        public string Description { get; set; }

        public int? CheckInID { get; set; }

        public string Nickname { get; set; }

        public string SID { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (TopicID == 0)
                results.Add(new ValidationResult("You must select a topic."));

            if (CheckInID.HasValue && CheckInID.Value == 0)
                results.Add(new ValidationResult("Invalid check in ID."));

            if (!StudentID.HasValue)
            {
                if (string.IsNullOrEmpty(Nickname) || string.IsNullOrEmpty(SID))
                    results.Add(new ValidationResult("You must choose a nick name."));
            }

            return results;
        }
    }
}
