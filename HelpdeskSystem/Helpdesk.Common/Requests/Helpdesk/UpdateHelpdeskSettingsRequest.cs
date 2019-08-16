using System;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Common.Requests.Helpdesk
{
    public class UpdateHelpdeskSettingsRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} cannot be empty!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} must be true or false!")]
        public bool HasCheckIn { get; set; }

        [Required(ErrorMessage = "{0} must be true or false!")]
        public bool HasQueue { get; set; }
    }
}
