using System;
using System.Collections.Generic;
using System.Net;
using Helpdesk.Common.Responses.Topics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Used to test topic related code
    /// </summary>
    [TestClass]
    public class TopicTests
    {
        // Recommend setting testEntityFactory.PopulateEmptyStrings = true; at the start of each method
        private readonly TestEntityFactory testEntityFactory = new TestEntityFactory();

        /// <summary>
        /// Used to test getting topics by their unit id works
        /// </summary>
        [TestMethod]
        public void GetTopicsByUnitID()
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

            // Get all topics associated with the unit id.
            var topicsFacade = new TopicsFacade();
            GetTopicsByUnitIDResponse topicResponse = topicsFacade.GetTopicsByUnitID(unitData.Response.UnitID);

            // Check that the topics retrieved are correct.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(topicResponse.Topics.Count == 3);
        }
    }
}
