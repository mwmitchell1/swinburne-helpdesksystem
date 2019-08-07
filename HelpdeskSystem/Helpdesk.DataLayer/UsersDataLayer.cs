using Helpdesk.Common.DTOs;
using Helpdesk.Data.Models;
using Helpdesk.Common.Requests.Users;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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

        public UserDTO GetUserByUsername(string username)
        {
            UserDTO dto = null;
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var user = context.User.FirstOrDefault(u => u.Username == username);
            }
            return dto;
        }

        public UserDTO DAO2DTO(User user)
        {
            var dto = new UserDTO()
            {
                UserID = user.UserId,
                Password = user.Password,
                Username = user.Username
            };

            return dto;
        }
    }
}
