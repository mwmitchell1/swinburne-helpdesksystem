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
                Assert.AreEqual(request.StudentID, checkInHistory.StudentId);

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
                Assert.AreEqual(request.StudentID, checkInHistory.StudentId);
            }
        }

        /// <summary>
        /// Tests checking in with a student id that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void CheckInExistingStudentNotFound()
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
                StudentID = -1
            };

            CheckInFacade facade = new CheckInFacade();

            CheckInResponse response = facade.CheckIn(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
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
        /// Tests checking in a with a new student with an existing nickname is handled properly
        /// </summary>
        [TestMethod]
        public void CheckInNewStudentExistingNickname()
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
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                Nickname = nickname.NickName
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

            CheckInRequest request = new CheckInRequest()
            {
                SID = nickname.Sid
            };

            CheckInFacade facade = new CheckInFacade();

            CheckInResponse response = facade.CheckIn(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Test checking out works with no forced checkout
        /// </summary>
        [TestMethod]
        public void CheckOutNotForced()
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

            CheckOutRequest coRequest = new CheckOutRequest()
            {
                ForcedCheckout = false
            };

            CheckOutResponse coResponse = facade.CheckOut(coRequest, response.CheckInID);

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

                Assert.AreEqual(coRequest.ForcedCheckout, checkOut.ForcedCheckout);
            }
        }

        /// <summary>
        /// Used to ensure that when checking out any queue items associated with that user are removed from the queue
        /// </summary>
        [TestMethod]
        public void CheckOutWithQueueItems()
        {
            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
                Name = AlphaNumericStringGenerator.GetString(10),
                IsDeleted = false
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Unit.Add(unit);
                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
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

            Queueitem queueitem = new Queueitem()
            {
                StudentId = response.StudentID,
                TopicId = topic.TopicId,
                Description = "Check In Unit Test",
                TimeAdded = DateTime.Now
            };



            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Queueitem.Add(queueitem);

                Checkinqueueitem checkinqueueitem = new Checkinqueueitem()
                {
                    CheckInId = response.CheckInID,
                    QueueItemId = queueitem.ItemId
                };
                context.Checkinqueueitem.Add(checkinqueueitem);

                context.SaveChanges();
            }

            CheckOutRequest coRequest = new CheckOutRequest()
            {
                ForcedCheckout = false
            };

            CheckOutResponse coResponse = facade.CheckOut(coRequest, response.CheckInID);

            Assert.AreEqual(HttpStatusCode.OK, coResponse.Status);
            Assert.IsTrue(coResponse.Result);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Checkinhistory checkOut = context.Checkinhistory.FirstOrDefault(co => co.CheckInId == response.CheckInID);
                Queueitem item = context.Queueitem.FirstOrDefault(qi => qi.ItemId == queueitem.ItemId);

                Assert.IsNotNull(checkOut);

                var baseTime = DateTime.Now.AddMinutes(-1);
                var addTime = checkOut.CheckoutTime;
                var timeDiff = baseTime.CompareTo(addTime);
                Assert.IsTrue(timeDiff == -1);

                Assert.AreEqual(coRequest.ForcedCheckout, checkOut.ForcedCheckout);

                Assert.IsNotNull(item);

                var baseQueueTime = DateTime.Now.AddMinutes(-1);
                var removeQueueTime = item.TimeRemoved;
                var queueTimeDiff = baseQueueTime.CompareTo(removeQueueTime);
                Assert.IsTrue(queueTimeDiff == -1);
            }
        }

        /// <summary>
        /// Test checking out works with forced checkout
        /// </summary>
        [TestMethod]
        public void CheckOutForced()
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

            CheckOutRequest coRequest = new CheckOutRequest()
            {
                ForcedCheckout = true
            };

            CheckOutResponse coResponse = facade.CheckOut(coRequest, response.CheckInID);

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

                Assert.AreEqual(coRequest.ForcedCheckout, checkOut.ForcedCheckout);
            }
        }

        /// <summary>
        /// Test checking out without a valid check in item is handled properly
        /// </summary>
        [TestMethod]
        public void CheckOutNoCheckInItem()
        {
            CheckInFacade facade = new CheckInFacade();

            CheckOutRequest request = new CheckOutRequest()
            {
                ForcedCheckout = false
            };

            CheckOutResponse coResponse = facade.CheckOut(request, -1);

            Assert.AreEqual(HttpStatusCode.NotFound, coResponse.Status);
        }

        /// <summary>
        /// This tests that the get checkins by helpdesk id works
        /// </summary>
        [TestMethod]
        public void GetCheckinsNoCheckedOut()
        {
            var helpdesk = new Helpdesksettings()
            {
                Name = AlphaNumericStringGenerator.GetString(8),
                HasCheckIn = true,
                IsDeleted = false
            };

            var unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(6),
                Name = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);

                context.SaveChanges();

                context.Helpdeskunit.Add(new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                });

                var student = new Nicknames()
                {
                    NickName = AlphaNumericStringGenerator.GetString(8),
                    Sid = AlphaNumericStringGenerator.GetStudentIDString()
                };

                var student2 = new Nicknames()
                {
                    NickName = AlphaNumericStringGenerator.GetString(8),
                    Sid = AlphaNumericStringGenerator.GetStudentIDString()
                };

                context.Nicknames.Add(student);
                context.Nicknames.Add(student2);

                context.SaveChanges();

                var checkIn = new Checkinhistory()
                {
                    UnitId = unit.UnitId,
                    StudentId = student.StudentId,
                    CheckInTime = DateTime.Now,
                };

                var checkIn2 = new Checkinhistory()
                {
                    UnitId = unit.UnitId,
                    StudentId = student2.StudentId,
                    CheckInTime = DateTime.Now,
                };

                context.Checkinhistory.Add(checkIn);
                context.Checkinhistory.Add(checkIn2);

                context.SaveChanges();
            }

            var facade = new CheckInFacade();
            var response = facade.GetCheckInsByHelpdeskId(helpdesk.HelpdeskId);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.CheckIns.Count == 2);
        }

        /// <summary>
        /// Makes sure that get checkins by helpdesk id only gets still checked in items
        /// </summary>
        [TestMethod]
        public void GetCheckinsTwoCheckedOut()
        {
            var checkIn = new Checkinhistory();
            var checkIn2 = new Checkinhistory();
            var checkIn3 = new Checkinhistory();

            var helpdesk = new Helpdesksettings()
            {
                Name = AlphaNumericStringGenerator.GetString(8),
                HasCheckIn = true,
                IsDeleted = false
            };

            var unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(6),
                Name = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);

                context.SaveChanges();

                context.Helpdeskunit.Add(new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                });

                var student = new Nicknames()
                {
                    NickName = AlphaNumericStringGenerator.GetString(8),
                    Sid = AlphaNumericStringGenerator.GetStudentIDString()
                };

                var student2 = new Nicknames()
                {
                    NickName = AlphaNumericStringGenerator.GetString(8),
                    Sid = AlphaNumericStringGenerator.GetStudentIDString()
                };

                var student3 = new Nicknames()
                {
                    NickName = AlphaNumericStringGenerator.GetString(8),
                    Sid = AlphaNumericStringGenerator.GetStudentIDString()
                };

                context.Nicknames.Add(student);
                context.Nicknames.Add(student2);
                context.Nicknames.Add(student3);

                context.SaveChanges();

                checkIn = new Checkinhistory()
                {
                    UnitId = unit.UnitId,
                    StudentId = student.StudentId,
                    CheckInTime = DateTime.Now,
                };

                context.Checkinhistory.Add(checkIn);

                checkIn2 = new Checkinhistory()
                {
                    UnitId = unit.UnitId,
                    StudentId = student2.StudentId,
                    CheckInTime = DateTime.Now,
                };

                checkIn3 = new Checkinhistory()
                {
                    UnitId = unit.UnitId,
                    StudentId = student3.StudentId,
                    CheckInTime = DateTime.Now,
                };
                context.Checkinhistory.Add(checkIn);
                context.Checkinhistory.Add(checkIn2);
                context.Checkinhistory.Add(checkIn3);

                context.SaveChanges();
            }

            var facade = new CheckInFacade();
            var response = facade.GetCheckInsByHelpdeskId(helpdesk.HelpdeskId);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.CheckIns.Count == 3);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                checkIn.CheckoutTime = DateTime.Now;
                checkIn3.ForcedCheckout = true;
                context.Update(checkIn);
                context.Update(checkIn3);
                context.SaveChanges();
            }

            response = facade.GetCheckInsByHelpdeskId(helpdesk.HelpdeskId);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.CheckIns.Count == 1);
        }
    }
}
