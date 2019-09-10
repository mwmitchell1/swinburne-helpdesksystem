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
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Used to test queue related code
    /// </summary>
    [TestClass]
    public class QueueTests
    {
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

                checkin.StudentId = nickname.StudentId;
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

        /// <summary>
        /// Test getting every queue item from a specific helpdesk from the database
        /// </summary>
        [TestMethod]
        public void GetQueueItemsByHelpdeskID()
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

                context.Helpdeskunit.Add(helpdeskunit);
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

            GetQueueItemsByHelpdeskIDResponse testResponse = facade.GetQueueItemsByHelpdeskID(helpdesk.HelpdeskId);

            Assert.AreEqual(HttpStatusCode.OK, testResponse.Status);
            Assert.AreEqual(request.StudentID, testResponse.QueueItems[0].StudentId);
        }

        /// <summary>
        /// Test getting every queue item from a specific helpdesk with no queue items from the database
        /// is handled properly
        /// </summary>
        [TestMethod]
        public void GetQueueItemsByHelpdeskIDNoUnits()
        {
            Helpdesksettings helpdesk = new Helpdesksettings()
            {
                HasQueue = true,
                HasCheckIn = false,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(10),
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Helpdesksettings.Add(helpdesk);
                context.SaveChanges();
            }

            QueueFacade facade = new QueueFacade();

            GetQueueItemsByHelpdeskIDResponse testResponse = facade.GetQueueItemsByHelpdeskID(helpdesk.HelpdeskId);

            Assert.AreEqual(HttpStatusCode.NotFound, testResponse.Status);
        }

        /// <summary>
        /// Test getting every queue item from a specific helpdesk that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void GetQueueItemsByHelpdeskIDNoHelpdesk()
        {
            QueueFacade facade = new QueueFacade();

            GetQueueItemsByHelpdeskIDResponse testResponse = facade.GetQueueItemsByHelpdeskID(-1);

            Assert.AreEqual(HttpStatusCode.NotFound, testResponse.Status);
        }
    }
}
