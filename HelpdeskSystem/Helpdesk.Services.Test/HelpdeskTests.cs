using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Common.Responses.Helpdesk;
using Helpdesk.Common.Utilities;
using Helpdesk.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helpdesk.Services.Test
{
    [TestClass]
    public class HelpdeskTests
    {
        private readonly TestEntityFactory testEntityFactory = new TestEntityFactory();

        /// <summary>
        /// Ensures that adding a helpdesk works
        /// </summary>
        [TestMethod]
        public void AddHelpdesk()
        {
            TestDataHelpdesk testEntityData = testEntityFactory.AddHelpdesk();

            Assert.AreEqual(HttpStatusCode.OK, testEntityData.Response.Status);
            Assert.IsTrue(testEntityData.Response.HelpdeskID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdesk = context.Helpdesksettings.FirstOrDefault(p => p.HelpdeskId == testEntityData.Response.HelpdeskID);

                Assert.IsNotNull(helpdesk);
            }
        }

        /// <summary>
        /// Ensures that the validation requires a name and prevents a helpdesk being added
        /// </summary>
        [TestMethod]
        public void AddHelpdeskNoName()
        {
            var request = new AddHelpdeskRequest()
            {
                HasCheckIn = false,
                HasQueue = true
            };

            var facade = new HelpdeskFacade();
            var response = facade.AddHelpdesk(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }

        [TestMethod]
        public void GetHelpdesks()
        {
            var factory = new TestEntityFactory();

            var data = factory.AddHelpdesk(AlphaNumericStringGenerator.GetString(10));

            var facade = new HelpdeskFacade();
            var response = facade.GetHelpdesks();

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.Helpdesks.Count > 0);
        }

        [TestMethod]
        public void GetActiveHelpdesks()
        {
            var factory = new TestEntityFactory();

            var hd1 = factory.AddHelpdesk(AlphaNumericStringGenerator.GetString(10));
            var hd2 = factory.AddHelpdesk(AlphaNumericStringGenerator.GetString(10));

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdesk = context.Helpdesksettings.FirstOrDefault(hd => hd.HelpdeskId == hd2.Response.HelpdeskID);

                helpdesk.IsDeleted = true;
                context.SaveChanges();
            }

            var facade = new HelpdeskFacade();
            var response = facade.GetActiveHelpdesks();

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.Helpdesks.Count > 0);

            List<int> helpdeskIds = response.Helpdesks.Select(hd => hd.HelpdeskID).ToList();

            Assert.IsTrue(helpdeskIds.Contains(hd1.Response.HelpdeskID));
            Assert.IsTrue(!helpdeskIds.Contains(hd2.Response.HelpdeskID));
        }

        [TestMethod]
        public void GetHelpdeskSuccess()
        {
            var factory = new TestEntityFactory();

            var data = factory.AddHelpdesk(AlphaNumericStringGenerator.GetString(10));

            var facade = new HelpdeskFacade();
            var response = facade.GetHelpdesk(data.Response.HelpdeskID);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsNotNull(response.Helpdesk);
        }

        [TestMethod]
        public void GetHelpdeskNotFound()
        {
            var facade = new HelpdeskFacade();
            var response = facade.GetHelpdesk(-1);

            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
            Assert.IsNull(response.Helpdesk);
        }


        /// <summary>
        /// Ensures that updating the helpdesk works correctly
        /// </summary>
        [TestMethod]
        public void UpdateHelpdesk()
        {
            var request = new AddHelpdeskRequest()
            {
                HasCheckIn = false,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            var facade = new HelpdeskFacade();
            var response = facade.AddHelpdesk(request);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsTrue(response.HelpdeskID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdesk = context.Helpdesksettings.FirstOrDefault(p => p.HelpdeskId == response.HelpdeskID);

                Assert.IsNotNull(helpdesk);
            }

            var updateRequest = new UpdateHelpdeskRequest()
            {
                HasCheckIn = true,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            var updateResponse = facade.UpdateHelpdesk(response.HelpdeskID, updateRequest);

            Assert.AreEqual(HttpStatusCode.OK, response.Status);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdesk = context.Helpdesksettings.FirstOrDefault(p => p.HelpdeskId == response.HelpdeskID);

                Assert.IsNotNull(helpdesk);
                Assert.AreEqual(updateRequest.HasCheckIn, helpdesk.HasCheckIn);
                Assert.AreEqual(updateRequest.HasQueue, helpdesk.HasQueue);
                Assert.AreEqual(updateRequest.Name, helpdesk.Name);
            }
        }

        /// <summary>
        /// Used to ensure that updating a helpdesk that does not exist, errors in the correct way
        /// </summary>
        [TestMethod]
        public void UpdateNotFoundHelpdesk()
        {
            var facade = new HelpdeskFacade();
            var updateRequest = new UpdateHelpdeskRequest()
            {
                HasCheckIn = true,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            var updateResponse = facade.UpdateHelpdesk(-1, updateRequest);

            Assert.AreEqual(HttpStatusCode.NotFound, updateResponse.Status);
        }

        /// <summary>
        /// Tests checking out and removing all queue items.
        /// </summary>
        [TestMethod]
        public void ForceCheckoutQueueRemove()
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

            // Get topics that were created and check that there are 2 (Layouts and Lifecycle).
            List<TopicDTO> unitTopics = testEntityFactory.TopicsFacade.GetTopicsByUnitID(unitData.Response.UnitID).Topics;
            Assert.IsTrue(unitTopics.Count == 2);

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            // Create students and check that they were created successfully.
            TestDataNickname nicknameDataA = testEntityFactory.AddNickname();
            Assert.AreEqual(HttpStatusCode.OK, nicknameDataA.Response.Status);
            Assert.IsTrue(nicknameDataA.Response.StudentID > 0);
            TestDataNickname nicknameDataB = testEntityFactory.AddNickname();
            Assert.AreEqual(HttpStatusCode.OK, nicknameDataB.Response.Status);
            Assert.IsTrue(nicknameDataB.Response.StudentID > 0);

            // Create two checkins and check that they're created successfully.
            TestDataCheckIn checkinDataA = testEntityFactory.AddCheckIn(nicknameDataA.Response.StudentID, null, null, unitData.Response.UnitID);
            Assert.AreEqual(HttpStatusCode.OK, checkinDataA.Response.Status);
            Assert.IsTrue(checkinDataA.Response.CheckInID > 0);
            TestDataCheckIn checkinDataB = testEntityFactory.AddCheckIn(nicknameDataB.Response.StudentID, null, null, unitData.Response.UnitID);
            Assert.AreEqual(HttpStatusCode.OK, checkinDataB.Response.Status);
            Assert.IsTrue(checkinDataB.Response.CheckInID > 0);

            // Create three queue items, 2 which are associated with checkinDataA and 1 with checkinDataB.
            TestDataQueue queueDataA = testEntityFactory.AddQueueItem(nicknameDataA.Response.StudentID,
                unitTopics[0].TopicId, checkinDataA.Response.CheckInID);
            Assert.AreEqual(HttpStatusCode.OK, queueDataA.Response.Status);
            Assert.IsTrue(queueDataA.Response.ItemId > 0);
            TestDataQueue queueDataB = testEntityFactory.AddQueueItem(nicknameDataA.Response.StudentID,
                unitTopics[0].TopicId, checkinDataA.Response.CheckInID);
            Assert.AreEqual(HttpStatusCode.OK, queueDataB.Response.Status);
            Assert.IsTrue(queueDataB.Response.ItemId > 0);
            TestDataQueue queueDataC = testEntityFactory.AddQueueItem(nicknameDataB.Response.StudentID,
                unitTopics[0].TopicId, checkinDataB.Response.CheckInID);
            Assert.AreEqual(HttpStatusCode.OK, queueDataC.Response.Status);
            Assert.IsTrue(queueDataC.Response.ItemId > 0);

            /*TODO -- Update with new checkout method
            // Manuall checkout checkinDataB and check that it succeeded.
            var checkoutBResponse = testEntityFactory.CheckInFacade.CheckOut(checkinDataB.Response.CheckInID);
            Assert.AreEqual(HttpStatusCode.OK, checkoutBResponse.Status);
            Assert.IsTrue(checkoutBResponse.Result == true);*/

            // Do the force checkout and queue remove.
            var forceCheckoutQueueRemoveResponse = testEntityFactory.HelpdeskFacade.ForceCheckoutQueueRemove();
            Assert.AreEqual(HttpStatusCode.OK, forceCheckoutQueueRemoveResponse.Status);
            Assert.IsTrue(forceCheckoutQueueRemoveResponse.Result == true);

            // Check all checkouts and and queue removals occured as expected.
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                // CheckinA was was not manually checked out, so ForcedCheckout should be true.
                var checkinA = context.Checkinhistory.FirstOrDefault(c => c.CheckInId == checkinDataA.Response.CheckInID);
                Assert.IsTrue(checkinA.ForcedCheckout == 1);
                Assert.IsTrue(checkinA.CheckoutTime != null);

                // CheckinB was manually checked out, so ForcedCheckout should be false.
                var checkinB = context.Checkinhistory.FirstOrDefault(c => c.CheckInId == checkinDataB.Response.CheckInID);
                Assert.IsTrue(checkinB.ForcedCheckout == 0);
                Assert.IsTrue(checkinB.CheckoutTime != null);

                // Check that all queue items were removed.
                var queueItemA = context.Queueitem.FirstOrDefault(q => q.ItemId == queueDataA.Response.ItemId);
                Assert.IsTrue(queueItemA.TimeRemoved != null);

                var queueItemB = context.Queueitem.FirstOrDefault(q => q.ItemId == queueDataB.Response.ItemId);
                Assert.IsTrue(queueItemB.TimeRemoved != null);

                var queueItemC = context.Queueitem.FirstOrDefault(q => q.ItemId == queueDataC.Response.ItemId);
                Assert.IsTrue(queueItemC.TimeRemoved != null);
            }
        }

        /// <summary>
        /// Tests adding a timespan to the database with a valid request.
        /// </summary>
        [TestMethod]
        public void AddTimespan()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            DateTime startDate = DateTime.Today;
            DateTime endDate = new DateTime(startDate.Year + 1, startDate.Month, startDate.Day, 0, 0, 0);

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest();
            addTimeSpanRequest.HelpdeskId = 1;
            addTimeSpanRequest.Name = AlphaNumericStringGenerator.GetString(10);
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);
        }

        /// <summary>
        /// Tests adding a timespan where the end date precedes the start date.
        /// </summary>
        [TestMethod]
        public void AddTimespanEndBeforeStart()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            DateTime startDate = DateTime.Today;
            DateTime endDate = startDate.AddYears(-1);

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest
            {
                HelpdeskId = 1,
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = startDate,
                EndDate = endDate
            };

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addTimeSpanResponse.Status);
        }

        /// <summary>
        /// Tests adding a timespan where the start date predates the year that this system became available (2019).
        /// </summary>
        [TestMethod]
        public void AddTimespanStartDatePredatesSystem()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest();
            addTimeSpanRequest.HelpdeskId = 1;
            addTimeSpanRequest.Name = AlphaNumericStringGenerator.GetString(10);
            DateTime startDate = new DateTime(2018, 1, 1, 0, 0, 0);
            DateTime endDate = DateTime.Today;
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addTimeSpanResponse.Status);
        }

        /// <summary>
        /// Test updating a specific timespan's name, start date and end date
        /// </summary>
        [TestMethod]
        public void UpdateTimespanFound()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest()
            {
                HelpdeskId = 1,
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),

            };

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);

            UpdateTimeSpanRequest updateTimespanRequest = new UpdateTimeSpanRequest()
            {
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = new DateTime(2019, 01, 01),
                EndDate = new DateTime(2019, 06, 01),
            };

            UpdateTimeSpanResponse updateTimespanResponse = helpdeskFacade.UpdateTimeSpan(addTimeSpanResponse.SpanId, updateTimespanRequest);

            Assert.AreEqual(HttpStatusCode.OK, updateTimespanResponse.Status);
            Assert.IsTrue(updateTimespanResponse.Result);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var timespan = context.Timespans.FirstOrDefault(u => u.SpanId == addTimeSpanResponse.SpanId);

                Assert.AreEqual(timespan.StartDate, updateTimespanRequest.StartDate);
                Assert.AreEqual(timespan.Name, updateTimespanRequest.Name);
                Assert.AreEqual(timespan.EndDate, updateTimespanRequest.EndDate);
            }
        }

        /// <summary>
        /// Test updating a timespan that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateTimespanNotFound()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            UpdateTimeSpanRequest updateTimespanRequest = new UpdateTimeSpanRequest()
            {
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = new DateTime(2019, 08, 01),
                EndDate = new DateTime(2019, 11, 01)
            };

            UpdateTimeSpanResponse updateTimespanResponse = helpdeskFacade.UpdateTimeSpan(-1, updateTimespanRequest);

            Assert.AreEqual(HttpStatusCode.NotFound, updateTimespanResponse.Status);
        }

        /// <summary>
        /// Test updating a specific timespan without any information is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateTimespanNoInformation()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest()
            {
                HelpdeskId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);

            UpdateTimeSpanRequest updateTimespanRequest = new UpdateTimeSpanRequest();

            UpdateTimeSpanResponse updateTimespanResponse = helpdeskFacade.UpdateTimeSpan(addTimeSpanResponse.SpanId, updateTimespanRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, updateTimespanResponse.Status);
        }

        /// <summary>
        /// Test updating a specific timespan without a name is handled properly
        /// </summary>
        [TestMethod]
        public void UpdateTimespanNoName()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest()
            {
                HelpdeskId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
                Name = AlphaNumericStringGenerator.GetString(10)
            };
            
            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);

            UpdateTimeSpanRequest updateTimespanRequest = new UpdateTimeSpanRequest()
            {
                StartDate = new DateTime(2019, 01, 01),
                EndDate = new DateTime(2019, 06, 01),
            };

            UpdateTimeSpanResponse updateTimespanResponse = helpdeskFacade.UpdateTimeSpan(addTimeSpanResponse.SpanId, updateTimespanRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, updateTimespanResponse.Status);
        }

        /// <summary>
        /// Test getting every timespan from the database
        /// </summary>
        [TestMethod]
        public void GetTimespans()
        {
            AddHelpdeskRequest addHelpdeskRequest = new AddHelpdeskRequest
            {
                HasCheckIn = false,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();
            AddHelpdeskResponse addHelpdeskResponse = helpdeskFacade.AddHelpdesk(addHelpdeskRequest);

            Assert.AreEqual(HttpStatusCode.OK, addHelpdeskResponse.Status);

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest
            {
                HelpdeskId = addHelpdeskResponse.HelpdeskID,
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);

            GetTimeSpansResponse getTimespansResponse = helpdeskFacade.GetTimeSpans();

            Assert.AreEqual(HttpStatusCode.OK, getTimespansResponse.Status);
            Assert.IsNotNull(getTimespansResponse.Timespans.Find(t => t.SpanId == addTimeSpanResponse.SpanId));

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var timespans = context.Timespans.ToList();

                Assert.IsNotNull(timespans);
            }
        }

        /// <summary>
        /// Test getting a specific timespan from the database by their span id
        /// </summary>
        [TestMethod]
        public void GetTimespanFound()
        {
            AddHelpdeskRequest addHelpdeskRequest = new AddHelpdeskRequest
            {
                HasCheckIn = false,
                HasQueue = true,
                Name = AlphaNumericStringGenerator.GetString(10)
            };

            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();
            AddHelpdeskResponse addHelpdeskResponse = helpdeskFacade.AddHelpdesk(addHelpdeskRequest);

            Assert.AreEqual(HttpStatusCode.OK, addHelpdeskResponse.Status);

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest
            {
                HelpdeskId = addHelpdeskResponse.HelpdeskID,
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);

            GetTimeSpanResponse getTimespanResponse = helpdeskFacade.GetTimeSpan(addTimeSpanResponse.SpanId);

            Assert.AreEqual(HttpStatusCode.OK, getTimespanResponse.Status);
            Assert.AreEqual(addTimeSpanRequest.Name, getTimespanResponse.Timespan.Name);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var timespan = context.Timespans.FirstOrDefault(t => t.SpanId == addTimeSpanResponse.SpanId);

                Assert.IsNotNull(timespan);
                Assert.AreEqual(addHelpdeskResponse.HelpdeskID, timespan.HelpdeskId);
                Assert.AreEqual(addTimeSpanRequest.Name, timespan.Name);
            }
        }

        /// <summary>
        /// Test getting a timespan that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void GetTimespanNotFound()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            GetTimeSpanResponse getTimespanResponse = helpdeskFacade.GetTimeSpan(-1);

            Assert.AreEqual(HttpStatusCode.NotFound, getTimespanResponse.Status);
        }

        /// <summary>
        /// Used to test the full database export code works as expected
        /// </summary>
        [TestMethod]
        public void GetDatabaseExport()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();
            bool result = helpdeskFacade.ExportDatabase();

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Ensures that the code can delete a timespan
        /// </summary>
        [TestMethod]
        public void DeleteTimespan()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest()
            {
                HelpdeskId = 1,
                Name = AlphaNumericStringGenerator.GetString(10),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1)
            };

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);

            DeleteTimeSpanResponse deleteResponse = helpdeskFacade.DeleteTimeSpan(addTimeSpanResponse.SpanId);

            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.Status);

            GetTimeSpanResponse response = helpdeskFacade.GetTimeSpan(addTimeSpanResponse.SpanId);

            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var timespan = context.Timespans.FirstOrDefault(ts => ts.SpanId == addTimeSpanResponse.SpanId);

                Assert.IsNull(timespan);
            }
        }

        /// <summary>
        /// Ensures that deleting a timespan that does not exist is handled properly
        /// </summary>
        [TestMethod]
        public void DeleteTimespanNotFound()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            DeleteTimeSpanResponse deleteResponse = helpdeskFacade.DeleteTimeSpan(-1);

            Assert.AreEqual(HttpStatusCode.OK, deleteResponse.Status);
        }
    }
}