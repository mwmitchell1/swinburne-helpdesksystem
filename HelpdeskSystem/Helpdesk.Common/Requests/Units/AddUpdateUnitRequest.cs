using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Units
{
    public class AddUpdateUnitRequest : BaseRequest
    {
        public int UnitID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must select a helpdesk to add the unit to.")]
        public int HelpdeskID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter in a unit name.")]
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
    }
}
