using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Students
{
    /// <summary>
    /// Used to indicate the result of adding a new student
    /// </summary>
    public class AddStudentResponse : BaseResponse
    {
        public int StudentID { get; set; }
    }
}
