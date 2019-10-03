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
        public int? StudentID { get; set; }

        public string Nickname { get; set; }

        public string SID { get; set; }

        public int UnitID { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (UnitID==0)
                results.Add(new ValidationResult("You must select a unit."));

            if (!StudentID.HasValue)
            {
                if (string.IsNullOrEmpty(Nickname) || string.IsNullOrEmpty(SID))
                    results.Add(new ValidationResult("You must choose a nickname."));
            }

            return results;
        }
    }
}
