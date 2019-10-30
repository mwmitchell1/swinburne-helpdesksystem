using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Units
{
    /// <summary>
    /// This request is used to either add a new unit or update an existing one
    /// </summary>
    public class AddUpdateUnitRequest : BaseRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "You must select a helpdesk to add the unit to.")]
        public int HelpdeskID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter in a unit name.")]
        [StringLength(50, ErrorMessage = "Name incorrect length")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter in a code name.")]
        [StringLength(8, ErrorMessage = "Code incorrect length", MinimumLength = 8)]
        public string Code { get; set; }

        public bool IsDeleted { get; set; }

        public List<string> Topics { get; set; }

        public AddUpdateUnitRequest()
        {
            Topics = new List<string>();
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            int i = 0;
            foreach(string s in Topics)
            {
                if(s.Length>50)
                {
                    i++;
                }
            }

            if(i>0)
            {
                results.Add(new ValidationResult("Topic names cannot be larger than 50 characters, you have " + i + " topics that break this rule"));
            }

            return results;
        }
    }
}
