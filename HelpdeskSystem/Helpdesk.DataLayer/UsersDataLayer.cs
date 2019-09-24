using Helpdesk.Common.DTOs;
using Helpdesk.Data.Models;
using Helpdesk.Common.Requests.Users;
using System;
using System.Collections.Generic;
using NLog;
using System.Text;
using System.Linq;
using Helpdesk.Common.Extensions;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Helpdesk.DataLayer
{
    /// <summary>
    /// Used to handle any databases interactions for users including CRUD, login and logout
    /// </summary>
    public class UsersDataLayer
    {
        private static Logger s_Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Used to add a user to the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public int? AddUser(AddUserRequest request)
        {
            int? userId = null;

            User user = new User();
            user.Username = request.Username;
            user.Password = request.Password;
            user.FirstTime = true;
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
            UserDTO userDTO = null;

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == id);

                if (user != null)
                    userDTO = DAO2DTO(user);
                else
                    throw new NotFoundException("Unable to find user.");
            }
            return userDTO;
        }

        /// <summary>
        /// This method retrieves a list of all the users in the database
        /// </summary>
        /// <returns>A list of users retrieved from the database</returns>
        public List<UserDTO> GetUsers()
        {
            List<UserDTO> userDTOs = new List<UserDTO>();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var users = context.User.ToList();

                if (users.Count == 0)
                    throw new NotFoundException("Unable to find users!");

                foreach (User user in users)
                {
                    if (user != null)
                    {
                        UserDTO userDTO = DAO2DTO(user);
                        userDTOs.Add(userDTO);
                    }
                }
            }
            return userDTOs;
        }

        /// <summary>
        /// Used to get a datatable with all of the helpdesk records
        /// </summary>
        /// <returns>Datatable with the helpdesk records</returns>
        public DataTable GetUsersAsDataTable()
        {
            DataTable users = new DataTable();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                DbConnection conn = context.Database.GetDbConnection();
                ConnectionState state = conn.State;

                try
                {
                    if (state != ConnectionState.Open)
                        conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "GetAllUsers";
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            users.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (state != ConnectionState.Closed)
                        conn.Close();
                }
            }

            return users;
        }

        /// <summary>
        /// Used to update the specified user in the databse with the request's information
        /// </summary>
        /// <param name="id">The UserId of the user to be updated</param>
        /// <param name="request">The request that contains the user's new information</param>
        /// <returns>A boolean that indicates whether the operation was a success</returns>
        public bool UpdateUser(int id, UpdateUserRequest request)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                User user = context.User.FirstOrDefault(u => u.UserId == id);

                if (user == null)
                   return false;

                user.Username = request.Username;

                if (!string.IsNullOrEmpty(request.Password))
                    user.Password = request.Password;

                user.FirstTime = false;

                context.SaveChanges();
            }
            return true;
        }

        /// <summary>
        /// Used to delete the specified user from the database
        /// </summary>
        /// <param name="id">The id of the user to be deleted</param>
        /// <returns>An indication of whether or not the deletion was successful</returns>
        public bool DeleteUser(int id)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == id);

                if (user == null)
                    throw new NotFoundException("User does not exist in the database");

                context.User.Remove(user);
                context.SaveChanges();
            }
            return true;
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
        private UserDTO DAO2DTO(User user)
        {
            UserDTO userDTO = null;

            userDTO = new UserDTO();
            userDTO.UserId = user.UserId;
            userDTO.Username = user.Username;
            userDTO.Password = user.Password;
            userDTO.FirstTime = user.FirstTime;

            return userDTO;
        }

        /// <summary>
        /// Converts the user DTO to a DAO to interact with the database
        /// </summary>
        /// <param name="user">The DTO for the user</param>
        /// <returns>The DAO for the user</returns>
        private User DTO2DAO(UserDTO userDTO)
        {
            User user = null;
            user = new User();
            user.UserId = userDTO.UserId;
            user.Username = userDTO.Username;
            user.Password = userDTO.Password;
            user.FirstTime = userDTO.FirstTime;

            return user;
        }
    }
}
