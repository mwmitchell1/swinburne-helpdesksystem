using Helpdesk.Common.DTOs;
using Helpdesk.Data.Models;
using Helpdesk.Common.Requests.Users;
using System;
using System.Collections.Generic;
using NLog;
using System.Text;
using System.Linq;
using Helpdesk.Common.Extensions;

namespace Helpdesk.DataLayer
{
    /// <summary>
    /// Used to handle any business logic relatedto users including CRUD, login and logout
    /// </summary>
    public class UsersDataLayer
    {
        private static Logger s_Logger = LogManager.GetCurrentClassLogger();

        public int? AddUser(AddUserRequest request)
        {
            int? userId = null;

            User user = new User();
            user.Username = request.Username;
            user.Password = request.Password;
            using (var context = new helpdesksystemContext())
            {
                context.User.Add(user);
                context.SaveChanges();
                userId = user.UserId;
            }
            return userId;
        }

        /// <summary>
        /// Used to retreve a user by their id
        /// </summary>
        /// <param name="id">The id of the user</param>
        /// <returns>The user DTO</returns>
        public UserDTO GetUser(int id)
        {
            UserDTO userDto = null;
            using (var context = new helpdesksystemContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == id);

                if (user != null)
                    userDto = DAO2DTO(user);
            }
            return userDto;
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

        /// <summary>
        /// Used to get a user by their username initially made for the login function
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <returns>The object that represents the user</returns>
        public UserDTO GetUserByUsername(string username)
        {
            UserDTO dto = null;
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var user = context.User.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    dto = DAO2DTO(user);
                }
            }

            return dto;
        }

        /// <summary>
        /// Converts the user DAO to a DTO to send to the front end
        /// </summary>
        /// <param name="user">The DAO for the user</param>
        /// <returns>The DTO for the user</returns>
        public UserDTO DAO2DTO(User user)
        {
            UserDTO userDTO = null;

            userDTO = new UserDTO();
            userDTO.UserId = user.UserId;
            userDTO.Username = user.Username;
            userDTO.Password = user.Password;

            return userDTO;
        }

        /// <summary>
        /// Converts the user DTO to a DAO to interact with the database
        /// </summary>
        /// <param name="user">The DTO for the user</param>
        /// <returns>The DAO for the user</returns>
        public User DTO2DAO(UserDTO userDTO)
        {
            User user = null;
            user = new User();
            user.UserId = userDTO.UserId;
            user.Username = userDTO.Username;
            user.Password = userDTO.Password;

            return user;
        }
    }
}
