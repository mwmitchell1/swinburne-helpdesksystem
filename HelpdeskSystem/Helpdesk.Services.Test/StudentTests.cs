using Helpdesk.Common.Requests.Students;
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
    [TestClass]
    public class StudentTests
    {
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
    }
}
