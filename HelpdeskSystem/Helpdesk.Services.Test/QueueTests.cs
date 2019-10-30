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
using Helpdesk.Common.Responses.Topics;
using Microsoft.EntityFrameworkCore;

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
                Description = "JoinQueueNoCheckInNewStudent Test"
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
                Description = "JoinQueueNoCheckOldStudent Test"
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
                CheckInID = checkin.CheckInId,
                Description = "JoinQueueCheckInOldStudent Test"
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
                    UnitId = unit.UnitId,
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
                Description = "JoinQueueNoStudentFail Test"
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
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                Description = "JoinQueueNoTopicFail Test",
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Used to ensure that joining the queue does not work without a description
        /// </summary>
        [TestMethod]
        public void JoinQueueNoDescriptionFail()
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

                topic.UnitId = unit.UnitId;

                Helpdeskunit helpdeskunit = new Helpdeskunit()
                {
                    HelpdeskId = helpdesk.HelpdeskId,
                    UnitId = unit.UnitId
                };

                context.Topic.Add(topic);
                context.Helpdeskunit.Add(helpdeskunit);

                context.SaveChanges();
            }

            AddToQueueRequest request = new AddToQueueRequest()
            {
                Nickname = AlphaNumericStringGenerator.GetString(10),
                SID = AlphaNumericStringGenerator.GetStudentIDString(),
                TopicID = topic.TopicId
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
                CheckInID = -100,
                Description = "JoinInvalidCheckNotFound Test"
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }

        /// <summary>
        /// Test updating a queue item (topic).
        /// </summary>
        [TestMethod]
        public void UpdateQueueItem()
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

            // Get topics for the unit that was just created.
            TopicsFacade topicsFacade = new TopicsFacade();
            GetTopicsByUnitIDResponse topicResponse = topicsFacade.GetTopicsByUnitID(unitData.Response.UnitID);

            // Check that there are two units in the response (Layouts, Lifecycle).
            Assert.IsTrue(topicResponse.Topics.Count() == 3);

            // Add test queue item, pass in topic [0].
            TestDataQueue queueData = testEntityFactory.AddQueueItem(null, topicResponse.Topics[0].TopicId);

            // Check that queue item was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, queueData.Response.Status);

            // Create request to alter queue item.
            var queueUpdateRequest = new UpdateQueueItemRequest
            {
                TopicID = topicResponse.Topics[0].TopicId,
                Description = "UpdateQueueItem Test"
            };

            // Update the queue item
            UpdateQueueItemResponse updateQueueResponse = testEntityFactory.QueueFacade.UpdateQueueItem(queueData.Response.ItemId, queueUpdateRequest);

            // Check that queue item was updated successfully.
            Assert.AreEqual(HttpStatusCode.OK, updateQueueResponse.Status);

            // Do another request to change to another topic
            queueUpdateRequest = new UpdateQueueItemRequest
            {
                TopicID = topicResponse.Topics[1].TopicId,
                Description = "UpdateQueueItem Test 2"
            };

            // Update the queue item again
            updateQueueResponse = testEntityFactory.QueueFacade.UpdateQueueItem(queueData.Response.ItemId, queueUpdateRequest);

            // Check that queue item was updated successfully.
            Assert.AreEqual(HttpStatusCode.OK, updateQueueResponse.Status);
        }

        /// <summary>
        /// Ensures attempting to update a queue item that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateQueueItemDoesNotExist()
        {
            // Invalid topic id shouldn't matter, as QueueID will be assessed first in the data layer.
            var request = new UpdateQueueItemRequest
            {
                TopicID = -1
            };
            var queueFacade = new QueueFacade();
            var response = queueFacade.UpdateQueueItem(-1, request);

            // Check that a request containing both TimeHelped and TimeRemoved is rejected.
            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }

        /// <summary>
        /// Ensures attempting to update a queue item without a topic is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateQueueItemTopicDoesNotExist()
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

            // Get topics for the unit that was just created.
            TopicsFacade topicsFacade = new TopicsFacade();
            GetTopicsByUnitIDResponse topicResponse = topicsFacade.GetTopicsByUnitID(unitData.Response.UnitID);

            // Check that there are two units in the response (Layouts, Lifecycle).
            Assert.IsTrue(topicResponse.Topics.Count() == 3);

            // Add test queue item, pass in topic [0].
            TestDataQueue queueData = testEntityFactory.AddQueueItem(null, topicResponse.Topics[0].TopicId);

            // Check that queue item was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, queueData.Response.Status);

            // Create request to alter queue item.
            // NOTICE This should fail, as there's no TopicID -1
            var queueUpdateRequest = new UpdateQueueItemRequest
            {
                TopicID = -1,
                Description = "UpdateQueueItemTopicDoesNotExist"
            };

            // Update the queue item
            UpdateQueueItemResponse updateQueueResponse = testEntityFactory.QueueFacade.UpdateQueueItem(queueData.Response.ItemId, queueUpdateRequest);

            // Check that queue item update failed due to non-existent TopicID.
            Assert.AreEqual(HttpStatusCode.NotFound, updateQueueResponse.Status);
        }

        /// <summary>
        /// Tests successfully updating a queue item status.
        /// </summary>
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

            // Get topics for the unit that was just created.
            TopicsFacade topicsFacade = new TopicsFacade();
            GetTopicsByUnitIDResponse topicResponse = topicsFacade.GetTopicsByUnitID(unitData.Response.UnitID);

            // Check that there are two units in the response (Layouts, Lifecycle).
            Assert.IsTrue(topicResponse.Topics.Count() == 3);

            // Add test queue item, pass in topic [0].
            TestDataQueue queueData = testEntityFactory.AddQueueItem(null, topicResponse.Topics[0].TopicId);

            // Check that queue item was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, queueData.Response.Status);

            // Create update request for TimeHelped.
            var queueUpdateRequest = new UpdateQueueItemStatusRequest
            {
                TimeHelped = DateTime.Now,
                TimeRemoved = null
            };

            // Update queue item TimeHelped.
            var queueUpdateResponse = testEntityFactory.QueueFacade.UpdateQueueItemStatus(queueData.Response.ItemId, queueUpdateRequest);

            // Check that queue item was updated successfully.
            Assert.AreEqual(HttpStatusCode.OK, queueUpdateResponse.Status);
            Assert.IsTrue(queueUpdateResponse.Result == true);

            // Create update request for TimeRemoved.
            DateTime timeRemoved = queueUpdateRequest.TimeHelped.Value.AddMinutes(1);
            queueUpdateRequest = new UpdateQueueItemStatusRequest
            {
                TimeHelped = null,
                TimeRemoved = timeRemoved
            };

            // Update queue item TimeRemoved.
            queueUpdateResponse = testEntityFactory.QueueFacade.UpdateQueueItemStatus(queueData.Response.ItemId, queueUpdateRequest);

            // Check that queue item was updated successfully.
            Assert.AreEqual(HttpStatusCode.OK, queueUpdateResponse.Status);
            Assert.IsTrue(queueUpdateResponse.Result == true);
        }

        /// <summary>
        /// Ensures attempting to update a queue item with no item status is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateQueueItemStatusDoesNotExist()
        {
            DateTime timeHelped = DateTime.Now;
            var request = new UpdateQueueItemStatusRequest
            {
                TimeHelped = timeHelped,
                TimeRemoved = null
            };
            var queueFacade = new QueueFacade();
            var response = queueFacade.UpdateQueueItemStatus(-1, request);

            // Check that a request containing both TimeHelped and TimeRemoved is rejected.
            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }

        /// <summary>
        /// Tests that a request containing neither TimeHelped or TimeRemoved is rejected.
        /// At least one property must be set.
        /// </summary>
        [TestMethod]
        public void UpdateQueueItemStatusBothTimesNull()
        {
            var request = new UpdateQueueItemStatusRequest
            {
                TimeHelped = null,
                TimeRemoved = null
            };
            var queueFacade = new QueueFacade();
            var response = queueFacade.UpdateQueueItemStatus(-1, request);

            // Check that a request containing both TimeHelped and TimeRemoved is rejected.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Tests that a request containing both TimeHelped and TimeRemoved is rejected.
        /// Only one property can be set per request.
        /// </summary>
        [TestMethod]
        public void UpdateQueueItemStatusBothTimesNotNull()
        {
            DateTime timeHelped = DateTime.Now;
            DateTime timeRemoved = timeHelped.AddMinutes(1);
            var request = new UpdateQueueItemStatusRequest{
                TimeHelped = timeHelped,
                TimeRemoved = timeRemoved
            };
            var queueFacade = new QueueFacade();
            var response = queueFacade.UpdateQueueItemStatus(-1, request);

            // Check that a request containing both TimeHelped and TimeRemoved is rejected.
            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        /// <summary>
        /// Ensures that attempting to update a queue item with a time removed that
        /// occurs before the time added is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateQueueItemStatusInvalidTimeRemoved()
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

            // Get topics for the unit that was just created.
            TopicsFacade topicsFacade = new TopicsFacade();
            GetTopicsByUnitIDResponse topicResponse = topicsFacade.GetTopicsByUnitID(unitData.Response.UnitID);

            // Check that there are two units in the response (Layouts, Lifecycle).
            Assert.IsTrue(topicResponse.Topics.Count() == 3);

            // Add test queue item, pass in topic [0].
            TestDataQueue queueData = testEntityFactory.AddQueueItem(null, topicResponse.Topics[0].TopicId);

            // Check that queue item was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, queueData.Response.Status);

            // Create update request for TimeHelped.
            var queueUpdateRequest = new UpdateQueueItemStatusRequest
            {
                TimeHelped = DateTime.Now,
                TimeRemoved = null
            };

            // Update queue item TimeHelped.
            var queueUpdateResponse = testEntityFactory.QueueFacade.UpdateQueueItemStatus(queueData.Response.ItemId, queueUpdateRequest);

            // Check that queue item was updated successfully.
            Assert.AreEqual(HttpStatusCode.OK, queueData.Response.Status);
            Assert.IsTrue(queueUpdateResponse.Result == true);

            // Create update request for TimeRemoved.
            // NOTICE we're providing a TimedRemoved time that precedes TimeHelped. This should fail.
            DateTime timeRemoved = queueUpdateRequest.TimeHelped.Value.AddMinutes(-1);
            queueUpdateRequest = new UpdateQueueItemStatusRequest
            {
                TimeHelped = null,
                TimeRemoved = timeRemoved
            };

            // Update queue item TimeRemoved.
            queueUpdateResponse = testEntityFactory.QueueFacade.UpdateQueueItemStatus(queueData.Response.ItemId, queueUpdateRequest);

            // Check that queue item was updated successfully.
            Assert.AreEqual(HttpStatusCode.BadRequest, queueUpdateResponse.Status);
            Assert.IsTrue(queueUpdateResponse.Result == false);
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
                Description = "GetQueueItemsByHelpdeskID Test"
            };

            QueueFacade facade = new QueueFacade();

            AddToQueueResponse response = facade.AddToQueue(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            GetQueueItemsByHelpdeskIDResponse testResponse = facade.GetQueueItemsByHelpdeskID(helpdesk.HelpdeskId);

            Assert.AreEqual(HttpStatusCode.OK, testResponse.Status);
            var itemIds = testResponse.QueueItems.Select(i => i.ItemId).ToList();

            Assert.IsTrue(itemIds.Contains(response.ItemId));
        }

        /// <summary>
        /// Test getting every queue item from a specific helpdesk with no queue items from the database
        /// is handled properly
        /// </summary>
        [TestMethod]
        public void GetQueueItemsByHelpdeskIDNoItems()
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
