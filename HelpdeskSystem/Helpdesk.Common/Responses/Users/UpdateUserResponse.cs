using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.Common.Responses.Users
{
    /// <summary>
    /// Used to return a bool representing if the update of a user was
    /// a success or not, and a status message if another issue occurs
    /// </summary>
    public class UpdateUserResponse : BaseResponse
    {
        public bool Result { get; set; }
    }
}
