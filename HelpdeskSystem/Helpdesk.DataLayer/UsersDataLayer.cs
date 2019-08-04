using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helpdesk.DataLayer
{
    public class UsersDataLayer
    {
        public int? AddUser(AddUserRequest request)
        {
            throw new NotImplementedException();
        }

        public UserDTO GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public List<UserDTO> GetUsers()
        {
            throw new NotImplementedException();
        }

        public bool UpdateUser(int id, UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public UserDTO GetUserByUsername(AddUserRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
