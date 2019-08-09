using System.Net;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Users;
using Helpdesk.Common.Responses.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            // NOTE: This test will fail if the user already exists!
            // Need to decide how to handle tests that conflict with previous tests.
            // WR: You can use a auto gen alpha string function, there is one on stack overflow
            // if you Google it.
            UsersFacade usersFacade = new UsersFacade();

            AddUserRequest addUserRequest = new AddUserRequest();
            addUserRequest.Username = "Timmy";
            addUserRequest.Password = "FellDownAWell1";
            addUserRequest.PasswordConfirm = "FellDownAWell1";

            AddUserResponse addUserResponse = usersFacade.AddUser(addUserRequest);

            Assert.AreEqual(HttpStatusCode.OK, addUserResponse.Status);
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
    }
}
