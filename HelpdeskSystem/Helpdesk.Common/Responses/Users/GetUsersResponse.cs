using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Users
{
    /// <summary>
    /// Used to return the list of all users in the database, and a status message
    /// </summary>
    public class GetUsersResponse : BaseResponse
    {
        public List<UserDTO> Users { get; set; }
    }
}
