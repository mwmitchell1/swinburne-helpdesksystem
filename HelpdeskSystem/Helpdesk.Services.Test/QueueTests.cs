using Helpdesk.Common.Requests.Queue;
using Helpdesk.Common.Responses.Queue;
using Helpdesk.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Data.Models;
using System.Net;
using System.Linq;
using NLog;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Used to test queue related code
    /// </summary>
    [TestClass]
    public class QueueTests
    {
        private readonly TestEntityFactory testEntityFactory = new TestEntityFactory();

        private static Logger s_logger = LogManager.GetCurrentClassLogger(); 

        /// <summary>
        /// Used to ensure that joining the queue works without a check in works for a new student
        /// </summary>
        [TestMethod]
        public void JoinQueueNoCheckInNewStudent()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = false,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.SaveChanges();

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.SaveChanges();

                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                TopicID = topic.TopicId,
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.ItemId > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem queueitem = context.Queueitem.FirstOrDefault(qi => qi.ItemId == response.ItemId);

                var baseTime = DateTime.Now.AddMinutes(-1);
                var addTime = queueitem.TimeAdded;
                Assert.AreEqual(request.TopicID, queueitem.TopicId);
                var timeDiff = baseTime.CompareTo(addTime);
                Assert.IsTrue(timeDiff == -1);

                Nicknames nicknames = context.Nicknames.FirstOrDefault(n => n.StudentId == queueitem.StudentId);

                Assert.AreEqual(request.Nickname, nicknames.NickName);
                Assert.AreEqual(request.SID, nicknames.Sid);
            }
        }

        /// <summary>
        /// Used to ensure that joining the queue with a checkin works for a new student
        /// </summary>
        [TestMethod]
        public void JoinQueueCheckInNewStudent()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = true,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Checkinhistory checkin = new Checkinhistory()
            {
                CheckInTime = DateTime.Now,
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.SaveChanges();


                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.Helpdeskunit.Add(helpdeskunit);

                checkin.UnitId = unit.UnitId;
                context.Checkinhistory.Add(checkin);

                context.SaveChanges();

                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                TopicID = topic.TopicId,
                CheckInID = checkin.CheckInId
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.ItemId > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem queueitem = context.Queueitem.FirstOrDefault(qi => qi.ItemId == response.ItemId);

                var baseTime = DateTime.Now.AddMinutes(-1);
                var addTime = queueitem.TimeAdded;
                Assert.AreEqual(request.TopicID, queueitem.TopicId);
                var timeDiff = baseTime.CompareTo(addTime);
                Assert.IsTrue(timeDiff == -1);

                Nicknames nicknames = context.Nicknames.FirstOrDefault(n => n.StudentId == queueitem.StudentId);

                Assert.AreEqual(request.Nickname, nicknames.NickName);
                Assert.AreEqual(request.SID, nicknames.Sid);

                Checkinqueueitem checkinqueueitem = context.Checkinqueueitem.FirstOrDefault(cqi => cqi.CheckInId == checkin.CheckInId && cqi.QueueItemId == queueitem.ItemId);
                Assert.IsNotNull(checkinqueueitem);
            }
        }

        /// <summary>
        /// Used to ensure that joining the queue works
        /// </summary>
        [TestMethod]
        public void JoinQueueNoCheckOldStudent()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = false,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
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
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.Nicknames.Add(nickname);
                context.SaveChanges();

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.SaveChanges();

                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                StudentID = nickname.StudentId,
                TopicID = topic.TopicId,
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.ItemId > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem queueitem = context.Queueitem.FirstOrDefault(qi => qi.ItemId == response.ItemId);

                var baseTime = DateTime.Now.AddMinutes(-1);
                var addTime = queueitem.TimeAdded;
                Assert.AreEqual(request.TopicID, queueitem.TopicId);
                var timeDiff = baseTime.CompareTo(addTime);
                Assert.IsTrue(timeDiff == -1);
            }
        }

        /// <summary>
        /// Used to ensure that joining the queue works
        /// </summary>
        [TestMethod]
        public void JoinQueueCheckInOldStudent()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = true,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Checkinhistory checkin = new Checkinhistory()
            {
                CheckInTime = DateTime.Now,
            };

            Nicknames nickname = new Nicknames()
            {
                NickName = AlphaNumericStringGenerator.GetString(10),
                Sid = AlphaNumericStringGenerator.GetStudentIDString()
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.Nicknames.Add(nickname);
                context.SaveChanges();

                checkin.UnitId = unit.UnitId;

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.Helpdeskunit.Add(helpdeskunit);

                checkin.UnitId = unit.UnitId;
                context.Checkinhistory.Add(checkin);

                context.SaveChanges();

                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                StudentID = nickname.StudentId,
                TopicID = topic.TopicId,
                CheckInID = checkin.CheckInId
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.ItemId > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem queueitem = context.Queueitem.FirstOrDefault(qi => qi.ItemId == response.ItemId);

                var baseTime = DateTime.Now.AddMinutes(-1);
                var addTime = queueitem.TimeAdded;
                Assert.AreEqual(request.TopicID, queueitem.TopicId);
                var timeDiff = baseTime.CompareTo(addTime);
                Assert.IsTrue(timeDiff == -1);

                Checkinqueueitem checkinqueueitem = context.Checkinqueueitem.FirstOrDefault(cqi => cqi.CheckInId == checkin.CheckInId && cqi.QueueItemId == queueitem.ItemId);
                Assert.IsNotNull(checkinqueueitem);
            }
        }

        /// <summary>
        /// Used to ensure that joining the queue does not work without student info
        /// </summary>
        [TestMethod]
        public void JoinQueueNoStudentFail()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = true,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.SaveChanges();

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.Helpdeskunit.Add(helpdeskunit);

                context.SaveChanges();

                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                TopicID = topic.TopicId,
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Used to ensure that joining the queue does not work without student info
        /// </summary>
        [TestMethod]
        public void JoinQueueNoTopicFail()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = true,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.SaveChanges();

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.Helpdeskunit.Add(helpdeskunit);

                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString()
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Used to ensure that joining the queue does not work without student info
        /// </summary>
        [TestMethod]
        public void JoinInvalidCheckinFail()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = true,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.SaveChanges();

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.Helpdeskunit.Add(helpdeskunit);

                context.SaveChanges();

                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                TopicID = topic.TopicId,
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                CheckInID = 0
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Used to ensure that joining the queue does not work without student info
        /// </summary>
        [TestMethod]
        public void JoinQueueInvalidTopicFail()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = true,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.SaveChanges();

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.Helpdeskunit.Add(helpdeskunit);

                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                TopicID = 0
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Used to ensure that joining the queue does not work without student info
        /// </summary>
        [TestMethod]
        public void JoinInvalidCheckNotFound()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = true,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Unit unit = new Unit()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            Topic topic = new Topic()
            {
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.Unit.Add(unit);
                context.SaveChanges();

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.Helpdeskunit.Add(helpdeskunit);

                context.SaveChanges();

                topic.UnitId = unit.UnitId;
                context.Topic.Add(topic);
                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                TopicID = topic.TopicId,
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                CheckInID = -100
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }
        /*
        [TestMethod]
        public void UpdateQueueItemStatus()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a unit. ID provided is 0, which will indicates creation of new helpdesk.
            List<string> topics = new List<string>(new string[] { "Layouts", "Lifecycle" });
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, "", "", false, topics);

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            UnitsFacade unitsFacade = new UnitsFacade();
            UnitDTO unitDTO = unitsFacade.GetUnit(unitData.Response.UnitID).Unit;
            Assert.IsTrue(unitDTO.)

            //TestDataQueue queueData = testEntityFactory.AddQueueItem(null, )
        }
        */
    }
}
