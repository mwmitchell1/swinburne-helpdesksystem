using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Users
{
    /// <summary>
    /// This request is used to add a new user
    /// </summary>
    public class AddUserRequest : BaseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} cannot be empty!")]
        [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string Username { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            return results;
        }
    }
}
