using System.Net;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Users;
using Helpdesk.Common.Responses.Users;
using Helpdesk.Common.Utilities;
using Helpdesk.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Helpdesk.Services.Test
{
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
        /// Test adding a user with an empty password string.
        /// </summary>
        [TestMethod]
        public void AddUserEmptyPassword()
        {
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest();
            addUserRequest.Username = AlphaNumericStringGenerator.GetString(10);
            addUserRequest.Password = "";
            addUserRequest.PasswordConfirm = "Password1";

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUserResponse.Status);
        }

        /// <summary>
        /// Test adding a user with an empty password confirmation string.
        /// </summary>
        [TestMethod]
        public void AddUserEmptyPasswordConfirm()
        {
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest();
            addUserRequest.Username = AlphaNumericStringGenerator.GetString(10);
            addUserRequest.Password = "Password1";
            addUserRequest.PasswordConfirm = "";

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUserResponse.Status);
        }

        /// <summary>
        /// Test adding a user where the password and password confirmation fields do not match.
        /// </summary>
        [TestMethod]
        public void AddUserPasswordMismatch()
        {
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest();
            addUserRequest.Username = AlphaNumericStringGenerator.GetString(10);
            addUserRequest.Password = "Password1";
            addUserRequest.PasswordConfirm = "Password2";

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

            DeleteUserResponse deleteResponse = usersFacade.DeleteUser(addUserResponse.UserId);

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

            DeleteUserResponse deleteResponse = usersFacade.DeleteUser(0);

            Assert.AreEqual(HttpStatusCode.NotFound, deleteResponse.Status);
        }

        /// <summary>
        /// Test getting every user from the database
        /// </summary>
        [TestMethod]
        public void GetUsers()
        {
            UsersFacade usersFacade = new UsersFacade();

            GetUsersResponse getUsersResponse = usersFacade.GetUsers();

            Assert.AreEqual(HttpStatusCode.OK, getUsersResponse.Status);
            Assert.AreEqual("Admin", getUsersResponse.Users[0].Username);

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

            GetUserResponse getUserResponse = usersFacade.GetUser(3);

            Assert.AreEqual(HttpStatusCode.NotFound, getUserResponse.Status);
        }

        /// <summary>
        /// Test updating a specific user's username and password
        /// </summary>
        [TestMethod]
        public void UpdateUserFound()
        {
            UsersFacade usersFacade = new UsersFacade();

            UpdateUserRequest updateUserRequest = new UpdateUserRequest()
            {
                Username = "UpdatedUser",
                Password = "UpdatedPassword"
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(6, updateUserRequest);

            Assert.AreEqual(HttpStatusCode.OK, updateUserResponse.Status);
            Assert.IsTrue(updateUserResponse.result);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var user = context.User.FirstOrDefault(u => u.UserId == 6);

                user.Username = "6WW2R9Y1F7";
                user.Password = "cMzZAHM41tgd07YnFiG5z5qX6gA=";

                context.SaveChanges();

                user = context.User.FirstOrDefault(u => u.UserId == 6);

                Assert.AreEqual(user.Username, "6WW2R9Y1F7");
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
                Username = "UpdatedUser",
                Password = "UpdatedPassword"
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(3, updateUserRequest);

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
                Password = "Password1"
            };

            UpdateUserResponse updateUserResponse = usersFacade.UpdateUser(8, updateUserRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, updateUserResponse.Status);
        }

    }
}
