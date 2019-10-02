using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.CheckIn
{
    /// <summary>
    /// This request is used to add a new check in item into the database
    /// </summary>
    public class CheckInRequest : BaseRequest
    {
        public int? SID { get; set; }

        public string Nickname { get; set; }

        public string StudentID { get; set; }

        public int UnitID { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (UnitID==0)
                results.Add(new ValidationResult("You must select a unit."));

            if (!SID.HasValue)
            {
                if (string.IsNullOrEmpty(Nickname) || string.IsNullOrEmpty(StudentID))
                    results.Add(new ValidationResult("You must choose a nickname."));
            }

            return results;
        }
    }
}
