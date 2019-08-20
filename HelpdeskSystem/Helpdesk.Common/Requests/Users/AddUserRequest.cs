using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Users
{
    public class AddUserRequest : BaseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} cannot be empty!")]
        [StringLength(20, ErrorMessage = "{0} cannot exceed {1} characters.")]
        public string Username { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} cannot be empty!")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} cannot be empty!")]
        public string PasswordConfirm { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!Password.Equals(PasswordConfirm))
            {
                results.Add(new ValidationResult("Password confirmation must match password!"));
            }

            return results;
        }
    }
}
