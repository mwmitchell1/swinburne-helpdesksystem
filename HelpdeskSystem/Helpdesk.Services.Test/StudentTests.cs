using Helpdesk.Common.Requests.Students;
using Helpdesk.Common.Responses.Students;
using Helpdesk.Common.Utilities;
using Helpdesk.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Used to test student and nickname related code
    /// </summary>
    [TestClass]
    public class StudentTests
    {
        /// <summary>
        /// Used to ensure adding a new student works
        /// </summary>
        [TestMethod]
        public void AddStudentNickname()
        {
            AddStudentRequest request = new AddStudentRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            var facade = new StudentFacade();
            var response = facade.AddStudentNickname(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.StudentID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var student = context.Nicknames.FirstOrDefault(p => p.StudentId == response.StudentID);
                Assert.IsNotNull(student);
            }
        }

        /// <summary>
        /// Used to ensure attempting to add a new student with an already existing
        /// nickname is handled properly
        /// </summary>
        [TestMethod]
        public void AddStudentNicknameExists()
        {
            AddStudentRequest request = new AddStudentRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            var facade = new StudentFacade();
            var response = facade.AddStudentNickname(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.StudentID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var student = context.Nicknames.FirstOrDefault(p => p.StudentId == response.StudentID);
                Assert.IsNotNull(student);
            }

            response = facade.AddStudentNickname(request);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Used to ensure getting a student by a specific nickname works properly
        /// </summary>
        [TestMethod]
        public void GetStudentByNickname()
        {
            AddStudentRequest request = new AddStudentRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            var facade = new StudentFacade();
            var response = facade.AddStudentNickname(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.StudentID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var student = context.Nicknames.FirstOrDefault(p => p.StudentId == response.StudentID);
                Assert.IsNotNull(student);
            }

            var getResponse = facade.GetStudentByNickname(request.Nickname);

            Assert.AreEqual(HttpStatusCode.OK, getResponse.Status);
            Assert.IsNotNull(getResponse.Nickname);
        }

        /// <summary>
        /// Test editing a student's nickname
        /// </summary>
        [TestMethod]
        public void EditStudentNickname()
        {
            AddStudentRequest request = new AddStudentRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            StudentFacade studentFacade = new StudentFacade();
            AddStudentResponse response = studentFacade.AddStudentNickname(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            EditStudentNicknameRequest editStudentNicknameRequest = new EditStudentNicknameRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10)
            };

            EditStudentNicknameResponse editStudentNicknameResponse = studentFacade.EditStudentNickname(response.StudentID, editStudentNicknameRequest);

            Assert.AreEqual(HttpStatusCode.OK, editStudentNicknameResponse.Status);
            Assert.IsTrue(editStudentNicknameResponse.Result);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var nickname = context.Nicknames.FirstOrDefault(n => n.StudentId == response.StudentID);

                Assert.AreEqual(editStudentNicknameRequest.Nickname, nickname.NickName);
            }
        }

        /// <summary>
        /// Test trying to edit a student who doesn't exists' nickname is handled properly
        /// </summary>
        [TestMethod]
        public void EditStudentNicknameNotFound()
        {
            StudentFacade studentFacade = new StudentFacade();

            EditStudentNicknameRequest editStudentNicknameRequest = new EditStudentNicknameRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
            };

            EditStudentNicknameResponse editStudentNicknameResponse = studentFacade.EditStudentNickname(-1, editStudentNicknameRequest);

            Assert.AreEqual(HttpStatusCode.NotFound, editStudentNicknameResponse.Status);
        }

        /// <summary>
        /// Test trying to edit a student's nickname with no nickname is handled properly
        /// </summary>
        [TestMethod]
        public void EditStudentNicknameNoNickname()
        {
            AddStudentRequest request = new AddStudentRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            StudentFacade studentFacade = new StudentFacade();
            AddStudentResponse response = studentFacade.AddStudentNickname(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            EditStudentNicknameRequest editStudentNicknameRequest = new EditStudentNicknameRequest()
            {
                Nickname = ""
            };

            EditStudentNicknameResponse editStudentNicknameResponse = studentFacade.EditStudentNickname(response.StudentID, editStudentNicknameRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, editStudentNicknameResponse.Status);
        }

        /// <summary>
        /// Test trying to edit a student's nickname with a nickname that already exists is handled properly
        /// </summary>
        [TestMethod]
        public void EditStudentNicknameExists()
        {
            AddStudentRequest request = new AddStudentRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            StudentFacade studentFacade = new StudentFacade();
            AddStudentResponse response = studentFacade.AddStudentNickname(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            EditStudentNicknameRequest editStudentNicknameRequest = new EditStudentNicknameRequest()
            {
                Nickname = request.Nickname
            };

            EditStudentNicknameResponse editStudentNicknameResponse = studentFacade.EditStudentNickname(response.StudentID, editStudentNicknameRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, editStudentNicknameResponse.Status);
        }

        /// <summary>
        /// Used to ensure the validation process to validate a new nickname works
        /// </summary>
        [TestMethod]
        public void ValidateNicknameNew()
        {
            ValidateNicknameRequest request = new ValidateNicknameRequest()
            {
                Name = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            var facade = new StudentFacade();

            var response = facade.ValidateNickname(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        /// <summary>
        /// Used to ensure validating an already existing student nickname works
        /// </summary>
        [TestMethod]
        public void ValidateNicknameOldValid()
        {
            Nicknames nickname = new Nicknames()
            {
                NickName = AlphaNumericStringGenerator.GetString(10),
                Sid = AlphaNumericStringGenerator.GetStudentIDString()
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Nicknames.Add(nickname);
                context.SaveChanges();
            }

            ValidateNicknameRequest request = new ValidateNicknameRequest()
            {
                Name = nickname.NickName,
                SID = nickname.Sid
            };

            var facade = new StudentFacade();

            var response = facade.ValidateNickname(request);

            Assert.AreEqual(HttpStatusCode.Accepted, response.Status);
        }

        /// <summary>
        /// Used to ensure the validation process detects an invalid existing nickname
        /// </summary>
        [TestMethod]
        public void ValidateNicknameOldInValid()
        {
            Nicknames nickname = new Nicknames()
            {
                NickName = AlphaNumericStringGenerator.GetString(10),
                Sid = AlphaNumericStringGenerator.GetStudentIDString()
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Nicknames.Add(nickname);
                context.SaveChanges();
            }

            ValidateNicknameRequest request = new ValidateNicknameRequest()
            {
                Name = nickname.NickName,
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            var facade = new StudentFacade();

            var response = facade.ValidateNickname(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }
    }
}
