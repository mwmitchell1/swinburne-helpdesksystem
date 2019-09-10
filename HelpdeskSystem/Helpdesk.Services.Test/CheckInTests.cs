using Helpdesk.Common.Requests.CheckIn;
using Helpdesk.Common.Responses.CheckIn;
using Helpdesk.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Data.Models;
using System.Net;
using System.Linq;
using NLog;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Used to test check in related code
    /// </summary>
    [TestClass]
    public class CheckInTests
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Test checking in with a new student works
        /// </summary>
        [TestMethod]
        public void CheckInNewStudent()
        {
            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Unit.Add(unit);
                context.SaveChanges();
            }

            CheckInRequest request = new CheckInRequest()
            {
                UnitID = unit.UnitId,
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            CheckInFacade facade = new CheckInFacade();

            CheckInResponse response = facade.CheckIn(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.CheckInID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Checkinhistory checkInHistory = context.Checkinhistory.FirstOrDefault(cih => cih.CheckInId == response.CheckInID);

                var baseTime = DateTime.Now.AddMinutes(-1);
                var addTime = checkInHistory.CheckInTime;
                Assert.AreEqual(request.UnitID, checkInHistory.UnitId);
                var timeDiff = baseTime.CompareTo(addTime);
                Assert.IsTrue(timeDiff == -1);

                Nicknames nicknames = context.Nicknames.FirstOrDefault(n => n.StudentId == checkInHistory.StudentId);

                Assert.AreEqual(request.Nickname, nicknames.NickName);
                Assert.AreEqual(request.SID, nicknames.Sid);
            }
        }

        /// <summary>
        /// Tests checking in with a student that already exists in the database works
        /// </summary>
        [TestMethod]
        public void CheckInExistingStudent()
        {
            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Nicknames nickname = new Nicknames()
            {
                NickName = AlphaNumericStringGenerator.GetString(10),
                Sid = AlphaNumericStringGenerator.GetStudentIDString()
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Unit.Add(unit);
                context.Nicknames.Add(nickname);
                context.SaveChanges();
            }

            CheckInRequest request = new CheckInRequest()
            {
                UnitID = unit.UnitId,
                StudentID = nickname.StudentId
            };

            CheckInFacade facade = new CheckInFacade();

            CheckInResponse response = facade.CheckIn(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.CheckInID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Checkinhistory checkInHistory = context.Checkinhistory.FirstOrDefault(cih => cih.CheckInId == response.CheckInID);

                var baseTime = DateTime.Now.AddMinutes(-1);
                var addTime = checkInHistory.CheckInTime;
                Assert.AreEqual(request.UnitID, checkInHistory.UnitId);
                var timeDiff = baseTime.CompareTo(addTime);
                Assert.IsTrue(timeDiff == -1);
            }
        }

        /// <summary>
        /// Tests checking in a with a new student with no nickname is handled properly
        /// </summary>
        [TestMethod]
        public void CheckInNewStudentNoNickname()
        {
            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Unit.Add(unit);
                context.SaveChanges();
            }

            CheckInRequest request = new CheckInRequest()
            {
                UnitID = unit.UnitId,
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            CheckInFacade facade = new CheckInFacade();

            CheckInResponse response = facade.CheckIn(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Tests checking in with a new student with no SID is handled properly
        /// </summary>
        [TestMethod]
        public void CheckInNewStudentNoSID()
        {
            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Unit.Add(unit);
                context.SaveChanges();
            }

            CheckInRequest request = new CheckInRequest()
            {
                UnitID = unit.UnitId,
                Nickname = AlphaNumericStringGenerator.GetString(10)
            };

            CheckInFacade facade = new CheckInFacade();

            CheckInResponse response = facade.CheckIn(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Test checking in without a unit is handled properly
        /// </summary>
        [TestMethod]
        public void CheckInNoUnit()
        {
            CheckInRequest request = new CheckInRequest();

            CheckInFacade facade = new CheckInFacade();

            CheckInResponse response = facade.CheckIn(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Test checking out works 
        /// </summary>
        [TestMethod]
        public void CheckOut()
        {
            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Unit.Add(unit);
                context.SaveChanges();
            }

            CheckInRequest request = new CheckInRequest()
            {
                UnitID = unit.UnitId,
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            CheckInFacade facade = new CheckInFacade();

            CheckInResponse response = facade.CheckIn(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.CheckInID > 0);

            CheckOutResponse coResponse = facade.CheckOut(response.CheckInID);

            Assert.AreEqual(HttpStatusCode.OK, coResponse.Status);
            Assert.IsTrue(coResponse.Result);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Checkinhistory checkOut = context.Checkinhistory.FirstOrDefault(co => co.CheckInId == response.CheckInID);

                Assert.IsNotNull(checkOut);

                var baseTime = DateTime.Now.AddMinutes(-1);
                var addTime = checkOut.CheckoutTime;
                var timeDiff = baseTime.CompareTo(addTime);
                Assert.IsTrue(timeDiff == -1);

                byte testValue = 0;

                Assert.AreEqual(testValue, checkOut.ForcedCheckout);
            }
        }

        /// <summary>
        /// Test checking out without a valid check in item is handled properly
        /// </summary>
        [TestMethod]
        public void CheckOutNoCheckInItem()
        {
            CheckInFacade facade = new CheckInFacade();

            CheckOutResponse coResponse = facade.CheckOut(-1);

            Assert.AreEqual(HttpStatusCode.NotFound, coResponse.Status);
        }
    }
}
