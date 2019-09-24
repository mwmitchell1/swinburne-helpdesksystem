using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Students
{
    /// <summary>
    /// Used to return a bool representing if the update of a student's nickname was
    /// a success or not, and a status message if another issue occurs
    /// </summary>
    public class EditStudentNicknameResponse : BaseResponse
    {
        public bool Result { get; set; }
    }
}
