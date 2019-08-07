﻿using Helpdesk.Common.DTOs;
using Helpdesk.Data.Models;
using Helpdesk.Common.Requests.Users;
using System;
using System.Collections.Generic;
using NLog;
using System.Text;
using System.Linq;

namespace Helpdesk.DataLayer
{
    public class UsersDataLayer
    {
        private static Logger s_Logger = LogManager.GetCurrentClassLogger();

        public int? AddUser(AddUserRequest request)
        {
            int? userId = null;

            User user = new User();
            user.Username = request.Username;
            user.Password = request.Password;
            using (var db = new helpdesksystemContext())
            {
                db.User.Add(user);
                db.SaveChanges();
                userId = user.UserId;
            }
            return userId;
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
                dto = DAO2DTO(user);
            }

            return dto;
        }

        public UserDTO DAO2DTO(User user)
        {
            UserDTO userDTO = null;

            userDTO = new UserDTO();
            userDTO.UserId = user.UserId;
            userDTO.Username = user.Username;
            userDTO.Password = user.Password;

            return userDTO;
        }

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
