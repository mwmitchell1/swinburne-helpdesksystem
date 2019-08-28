using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Helpdesk.Common.Requests.Students
{
    /// <summary>
    /// Used to add a studnet nickname
    /// </summary>
    public class AddStudentRequest : BaseRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter in a student ID")]
        public string SID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must enter in a nick name")]
        public string Nickname { get; set; }
    }
}
