using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Students
{
    /// <summary>
    ///Contains the new nickname for the specified student
    /// </summary>
    public class EditStudentNicknameRequest : BaseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter a nickname")]
        public string Nickname { get; set; }
    }
}
