using System.Net;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Users;
using Helpdesk.Common.Responses.Users;
using Helpdesk.Common.Utilities;
using Helpdesk.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Used to test user related code
    /// </summary>
    [TestClass]
    public class UserTests
    {
        /// <summary>
        /// Tests adding a user to the database with a valid request.
        /// </summary>
        [TestMethod]
        public void AddUser()
        {
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest();
            addUserRequest.Username = AlphaNumericStringGenerator.GetString(10);
            addUserRequest.Password = "Password1";
            addUserRequest.PasswordConfirm = "Password1";

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.OK, addUserResponse.Status);
        }

        /// <summary>
        /// Test that inputed username length is within the contraints set in the database.
        /// Limit currently set to 20 characters.
        /// </summary>
        [TestMethod]
        public void AddUserUsernameTooLong()
        {
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest();
            addUserRequest.Username = AlphaNumericStringGenerator.GetString(21);
            addUserRequest.Password = "Password1";
            addUserRequest.PasswordConfirm = "Password1";

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUserResponse.Status);
        }

        /// <summary>
        /// Test adding a user with an empty username string.
        /// </summary>
        [TestMethod]
        public void AddUserEmptyUsername()
        {
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest();
            addUserRequest.Username = "";
            addUserRequest.Password = "Password1";
            addUserRequest.PasswordConfirm = "Password1";

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUserResponse.Status);
        }

        /// <summary>
        /// Used to ensure that a user with valid credentials is logged in and receives a token
        /// </summary>
        [TestMethod]
        public void LoginUserValid()
        {
            LoginRequest request = new LoginRequest()
            {
                Password = "Password1",
                Username = "Admin"
            };

            var facade = new UsersFacade();
            var response = facade.LoginUser(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsFalse(string.IsNullOrEmpty(response.Token));
        }

        /// <summary>
        /// Used to ensure that a fake user is not logged in and does not receive a token
        /// </summary>
        [TestMethod]
        public void LoginFakeUser()
        {
            LoginRequest request = new LoginRequest()
            {
                Password = "Password1",
                Username = "fjQOFOJOIFJQO"
            };

            var facade = new UsersFacade();
            var response = facade.LoginUser(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
            Assert.IsTrue(string.IsNullOrEmpty(response.Token));
        }

        /// <summary>
        /// Used to ensure that a user is not logged in and does not receive a token
        /// when they are missing information
        /// </summary>
        [TestMethod]
        public void LoginUserInvalid()
        {
            LoginRequest request = new LoginRequest()
            {
                Password = "Password1",
                Username = ""
            };

            var facade = new UsersFacade();
            var response = facade.LoginUser(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
            Assert.IsTrue(string.IsNullOrEmpty(response.Token));
        }

        /// <summary>
        /// Used to ensure that a password check is working
        /// when they are missing information
        /// </summary>
        [TestMethod]
        public void LoginUserBadPassword()
        {
            LoginRequest request = new LoginRequest()
            {
                Password = "sggsgaeqQEGEVWS",
                Username = "Admin"
            };

            var facade = new UsersFacade();
            var response = facade.LoginUser(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
            Assert.IsTrue(string.IsNullOrEmpty(response.Token));
        }

        /// <summary>
        /// Ensures that the code can delete a user
        /// </summary>
        [TestMethod]
        public void DeleteUser()
        {
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest();
            addUserRequest.Username = AlphaNumericStringGenerator.GetString(10);
            addUserRequest.Password = "Password1";
            addUserRequest.PasswordConfirm = "Password1";

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.OK, addUserResponse.Status);

            DeleteUserResponse deleteResponse = usersFacade.DeleteUser(addUserResponse.UserId, "Admin");

            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.Status);

            GetUserResponse response = usersFacade.GetUser(addUserResponse.UserId);

            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }

        /// <summary>
        /// Ensures that deleting a user that does not exist is handled properly
        /// </summary>
        [TestMethod]
        public void DeleteUserNotFound()
        {
            UsersFacade usersFacade = new UsersFacade();

            DeleteUserResponse deleteResponse = usersFacade.DeleteUser(-1, "Admin");

            Assert.AreEqual(HttpStatusCode.NotFound, deleteResponse.Status);
        }

        /// <summary>
        /// Test getting every user from the database
        /// </summary>
        [TestMethod]
        public void GetUsers()
        {
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(10),
                Password = AlphaNumericStringGenerator.GetString(10),
                PasswordConfirm = AlphaNumericStringGenerator.GetString(10)
            };

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.OK, addUserResponse.Status);

            GetUsersResponse getUsersResponse = usersFacade.GetUsers();

            Assert.AreEqual(HttpStatusCode.OK, getUsersResponse.Status);
            Assert.IsNotNull(getUsersResponse.Users.Find(u => u.UserId == addUserResponse.UserId));

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var users = context.User.ToList();

                Assert.IsNotNull(users);
            }
        }

        /// <summary>
        /// Test getting a specific user from the database by their user id
        /// </summary>
        [TestMethod]
        public void GetUserFound()
        {
            UsersFacade usersFacade = new UsersFacade();

            GetUserResponse getUserResponse = usersFacade.GetUser(1);

            Assert.AreEqual(HttpStatusCode.OK, getUserResponse.Status);
            Assert.AreEqual("Admin", getUserResponse.User.Username);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == 1);

                Assert.IsNotNull(user);
            }
        }

        /// <summary>
        /// Test getting a user that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void GetUserNotFound()
        {
            UsersFacade usersFacade = new UsersFacade();

            GetUserResponse getUserResponse = usersFacade.GetUser(-1);

            Assert.AreEqual(HttpStatusCode.NotFound, getUserResponse.Status);
        }

        /// <summary>
        /// Test updating a specific user's username and password
        /// </summary>
        [TestMethod]
        public void UpdateUserFound()
        {

            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(10),
                Password = AlphaNumericStringGenerator.GetString(10),
                PasswordConfirm = AlphaNumericStringGenerator.GetString(10)
            };

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.OK, addUserResponse.Status);

            UpdateUserRequest updateUserRequest = new UpdateUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(10),
                Password = AlphaNumericStringGenerator.GetString(10)
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(addUserResponse.UserId, updateUserRequest);

            Assert.AreEqual(HttpStatusCode.OK, updateUserResponse.Status);
            Assert.IsTrue(updateUserResponse.Result);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == addUserResponse.UserId);

                user = context.User.FirstOrDefault(u => u.UserId == addUserResponse.UserId);

                Assert.AreEqual(updateUserRequest.Username, user.Username);
                Assert.AreEqual(updateUserRequest.Password, user.Password);
            }
        }

        /// <summary>
        /// Test trying to update a user that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateUserNotFound()
        {
            UsersFacade usersFacade = new UsersFacade();

            UpdateUserRequest updateUserRequest = new UpdateUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(9),
                Password = AlphaNumericStringGenerator.GetString(9)
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(-1, updateUserRequest);

            Assert.AreEqual(HttpStatusCode.NotFound, updateUserResponse.Status);
        }

        /// <summary>
        /// Test trying to update a user with a username that is too long is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateUserUsernameTooLong()
        {
            UsersFacade usersFacade = new UsersFacade();

            UpdateUserRequest updateUserRequest = new UpdateUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(21),
                Password = AlphaNumericStringGenerator.GetString(9)
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(8, updateUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, updateUserResponse.Status);
        }

        /// <summary>
        /// Test trying to update a user with a password that is too short is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateUserPasswordTooShort()
        {
            UsersFacade usersFacade = new UsersFacade();

            UpdateUserRequest updateUserRequest = new UpdateUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(10),
                Password = AlphaNumericStringGenerator.GetString(5),
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(8, updateUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, updateUserResponse.Status);
        }

        /// <summary>
        /// Test updating a specific user's username with a username that already exists is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateUserUsernameExists()
        {

            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(10),
                Password = AlphaNumericStringGenerator.GetString(10),
                PasswordConfirm = AlphaNumericStringGenerator.GetString(10)
            };

            AddUserRequest addUserRequest2 = new AddUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(10),
                Password = AlphaNumericStringGenerator.GetString(10),
                PasswordConfirm = AlphaNumericStringGenerator.GetString(10)
            };

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);
            AddUserResponse addUserResponse2 = usersFacade.AddUser(addUserRequest2);

            Assert.AreEqual(HttpStatusCode.OK, addUserResponse.Status);

            UpdateUserRequest updateUserRequest = new UpdateUserRequest()
            {
                Username = addUserResponse.Username,
                Password = AlphaNumericStringGenerator.GetString(10)
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(addUserResponse2.UserId, updateUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, updateUserResponse.Status);
        }

        /// <summary>
        /// Test updating a specific user's username with no username is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateUserNoUsername()
        {

            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest()
            {
                Username = AlphaNumericStringGenerator.GetString(10),
                Password = AlphaNumericStringGenerator.GetString(10),
                PasswordConfirm = AlphaNumericStringGenerator.GetString(10)
            };

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.OK, addUserResponse.Status);

            UpdateUserRequest updateUserRequest = new UpdateUserRequest()
            {
                Password = AlphaNumericStringGenerator.GetString(10)
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(addUserResponse.UserId, updateUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, updateUserResponse.Status);
        }

        /// <summary>
        /// Used to hash passwords when a user logs in, is added to the system or has their password changed
        /// </summary>
        /// <param name="text">The password in plain text</param>
        /// <returns>The hashed password</returns>
        private string HashText(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            var sha1 = new SHA1CryptoServiceProvider();
            var sha1data = sha1.ComputeHash(bytes);
            return Convert.ToBase64String(sha1data);
        }
    }
}
