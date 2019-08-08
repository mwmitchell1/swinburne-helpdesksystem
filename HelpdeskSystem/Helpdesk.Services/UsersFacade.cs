﻿using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Users;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Users;
using Helpdesk.DataLayer;
using NLog;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Helpdesk.Common.Extensions;

namespace Helpdesk.Services
{
    /// <summary>
    /// This class is used to handle the business logic of users
    /// </summary>
    public class UsersFacade : ILoginClass
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        public UsersFacade()
        {
            _appSettings = new AppSettings();
        }

        public GetUsersResponse GetUsers()
        {
            throw new NotImplementedException();
        }

        public GetUserResponse GetUser(int id)
        {
            throw new NotImplementedException();
        }

        public AddUserResponse AddUser(AddUserRequest request)
        {
            s_logger.Info("Adding user...");

            AddUserResponse response = new AddUserResponse();

            try
            {
                response = (AddUserResponse)request.CheckValidation(response);
                if (response.Status == HttpStatusCode.BadRequest)
                {
                    return response;
                }
                request.Password = HashText(request.Password);

                var dataLayer = new UsersDataLayer();

                if (dataLayer.GetUserByUsername(request.Username) != null)
                {
                    throw new Exception("Unable to add user! User already exists!");
                }

                int? result = dataLayer.AddUser(request);

                if (result == null)
                {
                    throw new Exception("Unable to add user!");
                }

                response.UserId = (int)result;
                response.Status = HttpStatusCode.OK;
            }
            catch(Exception ex)
            {
                s_logger.Error(ex, "Unable to add user!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add user!"));
            }
            return response;
        }

        public UpdateUserResponse UpdateUser(int id, UpdateUserRequest request)
        {
            throw new NotImplementedException();
        }

        public DeleteUserResponse DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public LoginResponse LoginUser(LoginRequest request)
        {
            s_logger.Info("Attempting to log in.");

            LoginResponse response = new LoginResponse();

            try
            {

                response = (LoginResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                var dataLayer = new UsersDataLayer();
                UserDTO user = dataLayer.GetUserByUsername(request.Username);

                if (user == null)
                {
                    response.Token = string.Empty;
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Unable to login username or password is incorrect"));
                    s_logger.Warn($"Unable to login as a username [ {request.Username} ], username or password is incorrect.");
                    return response;
                }

                string hashedPassword = HashText(request.Password);

                if (user.Password != hashedPassword)
                {
                    response.Token = string.Empty;
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Unable to login username or password is incorrect"));
                    s_logger.Warn($"Unable to login as a username [ {request.Username} ], username or password is incorrect.");
                    return response;
                }


                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.AppSecret);

                var tokenDescriptior = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Sid, user.UserId.ToString())
                    }),
                    Expires = DateTime.Now.AddYears(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };

                var rawToken = tokenHandler.CreateToken(tokenDescriptior);
                var token = tokenHandler.WriteToken(rawToken);

                response.Token = token;
                response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to perform log in attempt.");
                response = new LoginResponse
                {
                    Token = string.Empty,
                    Status = HttpStatusCode.InternalServerError
                };
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to perform log in attempt."));
            }
            return response;
        }

        public bool VerifyUser(string username, string userId)
        {
            bool result = false;

            try
            {
                var dataLayer = new UsersDataLayer();
                int userID = -1;

                if (!int.TryParse(userId, out userID))
                    throw new Exception("Invalid user id received.");

                UserDTO userFromID = dataLayer.GetUser(userID);

                UserDTO userFromUsername = dataLayer.GetUserByUsername(username);

                if (!(userFromID.UserId == userFromUsername.UserId && userFromID.Username == userFromUsername.Username))
                {
                    s_logger.Warn("Unable to verify user.");
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            catch (NotFoundException ex)
            {
                s_logger.Warn(ex, "Unable to find user in system.");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to perform log in attempt.");
            }
            return result;
        }

        private string HashText(string text)
        {
            string result = string.Empty;
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(text);
                var sha1 = new SHA1CryptoServiceProvider();
                var sha1data = sha1.ComputeHash(bytes);
                result = Convert.ToBase64String(sha1data);
            }
            catch (Exception ex)
            {
                result = string.Empty;
                s_logger.Error(ex, "Unable to hash text");
            }
            return result;
        }
    }
}
