using Helpdesk.Common;
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

        /// <summary>
        /// This method is responsible for retrieving all users from the helpdesk system
        /// </summary>
        /// <returns>The response that indicates if the operation was a success,
        /// and the list of users</returns>
        public GetUsersResponse GetUsers()
        {
            s_logger.Info("Getting users...");

            GetUsersResponse response = new GetUsersResponse();

            try
            {
                var dataLayer = new UsersDataLayer();

                List<UserDTO> users = dataLayer.GetUsers();

                response.Users = users;
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Error(ex, "No users found!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "No users found!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to get users!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get users!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for getting a specific user from the helpdesk system
        /// </summary>
        /// <param name="id">The UserId of the specific user to be retrieved</param>
        /// <returns>The response that indicates if the operation was a success,
        /// and the details of the retrieved user if it was</returns>
        public GetUserResponse GetUser(int id)
        {
            s_logger.Info("Getting user...");

            GetUserResponse response = new GetUserResponse();

            try
            {
                var dataLayer = new UsersDataLayer();

                UserDTO user = dataLayer.GetUser(id);

                if (user == null)
                    throw new NotFoundException("Unable to find user!");
              
                response.User = user;
                response.Status = HttpStatusCode.OK;

            }
            catch (NotFoundException ex)
            {
                s_logger.Error(ex, "Unable to find user!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to find user!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to get user!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get user!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for adding a new user.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

                if (string.IsNullOrEmpty(request.Password))
                    request.Password = request.Username;

                request.Password = HashText(request.Password);

                var dataLayer = new UsersDataLayer();

                if (dataLayer.GetUserByUsername(request.Username) != null)
                {
                    response.Status = HttpStatusCode.Forbidden;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.Forbidden, "Username already exists"));
                    return response;
                }

                int? result = dataLayer.AddUser(request);

                if (result == null)
                {
                    throw new Exception("Unable to add user!");
                }

                response.UserId = (int)result;
                response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to add user!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add user!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for updating a specific user's information, such
        /// as their username and password
        /// </summary>
        /// <param name="id">The UserId of the user to be updated</param>
        /// <param name="request">The user's new information</param>
        /// <returns>The response that indicates if the update was successfull</returns>
        public UpdateUserResponse UpdateUser(int id, UpdateUserRequest request)
        {
            s_logger.Info("Updating user...");

            UpdateUserResponse response = new UpdateUserResponse();

            try
            {
                response = (UpdateUserResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                request.Password = HashText(request.Password);

                var dataLayer = new UsersDataLayer();

                if (dataLayer.GetUserByUsername(request.Username)!=null && dataLayer.GetUserByUsername(request.Username).UserId != id)
                {
                    throw new Exception("Unable to update user! User with username " + request.Username + "already exists!");
                }

                bool result = dataLayer.UpdateUser(id, request);

                if (result == false)
                    throw new NotFoundException("Unable to find user!");

                response.Result = result;
                response.Status = HttpStatusCode.OK;

            }
            catch(NotFoundException ex)
            {
                s_logger.Error(ex, "Unable to find user!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to find user!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to update user!");
                response.Status = HttpStatusCode.Forbidden;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.Forbidden, "Unable to update user!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for handling the deletion of a user from the system
        /// </summary>
        /// <param name="id">The id of the user to be deleted</param>
        /// <returns>A response that indicates whether or not the deletion was successful</returns>
        public DeleteUserResponse DeleteUser(int id, string currentUser)
        {
            var response = new DeleteUserResponse();

            try
            {
                var dataLayer = new UsersDataLayer();

                UserDTO user = dataLayer.GetUser(id);

                if (user.Username == currentUser)
                {
                    response.Status = HttpStatusCode.Forbidden;
                    return response;
                }

                bool result = dataLayer.DeleteUser(id);

                if (result)
                    response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Warn($"Unable to find the user with id [{id}]");
                response.Status = HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to delete the user.");
                response.Status = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        /// <summary>
        /// This method is responsable for handling the validation and verification of the users login attempt
        /// </summary>
        /// <param name="request">the users login information</param>
        /// <returns>The response which indicates if they are sucessful and the bearer token
        /// they will use for authentication on success</returns>
        public LoginResponse LoginUser(LoginRequest request)
        {
            s_logger.Info("Attempting to log in...");

            LoginResponse response = new LoginResponse();

            try
            {
                //Validate input
                response = (LoginResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                var dataLayer = new UsersDataLayer();

                //Verify user exists
                UserDTO user = dataLayer.GetUserByUsername(request.Username);
                if (user == null)
                {
                    response.Token = string.Empty;
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Unable to login username or password is incorrect"));
                    s_logger.Warn($"Unable to login as a username [ {request.Username} ], username or password is incorrect.");
                    return response;
                }

                // Ensure that their password is correct
                string hashedPassword = HashText(request.Password);
                if (user.Password != hashedPassword)
                {
                    response.Token = string.Empty;
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Unable to login username or password is incorrect"));
                    s_logger.Warn($"Unable to login as a username [ {request.Username} ], username or password is incorrect.");
                    return response;
                }

                // Generate users bearer token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.AppSecret);

                var tokenDescriptior = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(new Claim[] {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Sid, user.UserId.ToString())
                    }),
                    Expires = DateTime.Now.AddHours(4),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
                };

                var rawToken = tokenHandler.CreateToken(tokenDescriptior);
                var token = tokenHandler.WriteToken(rawToken);

                response.Token = token;

                if (user.FirstTime)
                {
                    response.Status = HttpStatusCode.Accepted;
                    response.UserId = user.UserId;
                    return response;
                }

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

        /// <summary>
        /// This is used to check that the user that is logged in is actually a valid user in the system 
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <param name="userId">The id of the user</param>
        /// <returns>An indicator of whether or not the user is valid</returns>
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

                if (!(userFromID.UserId == userFromUsername.UserId && userFromID.Username == userFromUsername.Username && (!userFromID.FirstTime)))
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

        /// <summary>
        /// Used to hash passwords when a user logs in, is added to the system or has their password changed
        /// </summary>
        /// <param name="text">The password in plain text</param>
        /// <returns>The hashed password</returns>
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
