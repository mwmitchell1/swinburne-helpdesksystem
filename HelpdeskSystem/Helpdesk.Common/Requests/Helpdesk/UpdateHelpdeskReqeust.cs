using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Helpdesk
{
    /// <summary>
    /// This request is used to update a helpdesk
    /// </summary>
    public class UpdateHelpdeskRequest : BaseRequest
    {
        [Required (AllowEmptyStrings = false, ErrorMessage = "You must enter in a helpdesk name.")]
        public string Name { get; set; }

        public bool HasCheckIn { get; set; }

        public bool HasQueue { get; set; }
    }
}
