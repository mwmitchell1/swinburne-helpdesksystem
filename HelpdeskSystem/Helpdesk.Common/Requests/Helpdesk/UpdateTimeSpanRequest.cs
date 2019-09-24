using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Helpdesk
{
    /// <summary>
    /// Contains the timespan's new information
    /// </summary>
    public class UpdateTimeSpanRequest : BaseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter in a timespan name.")]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
