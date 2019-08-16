using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Helpdesk
{
    public class AddTimeSpanRequest : BaseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} cannot be empty!")]
        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (StartDate >= EndDate)
            {
                results.Add(new ValidationResult("Start date must be an earlier date than end date!"));
            }

            DateTime minDateTime = new DateTime(2019, 1, 1, 0, 0, 0);
            if (StartDate < minDateTime)
            {
                results.Add(new ValidationResult("Start date must be an earlier date than " + minDateTime.ToString() + "!"));
            }

            return results;
        }
    }
}
