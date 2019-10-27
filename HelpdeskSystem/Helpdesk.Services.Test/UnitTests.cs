using System;
using System.Net;
using System.Linq;
using Helpdesk.Services;
using Helpdesk.Common.Responses.Units;
using Helpdesk.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Helpdesk.Common.Requests.Units;
using System.Collections.Generic;
using Helpdesk.Common.Utilities;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Common.Responses.Helpdesk;

namespace Helpdesk.Services.Test
{
    /// <summary>
    /// Used to test unit related code
    /// </summary>
    [TestClass]
    public class UnitTests
    {
        // Recommend setting testEntityFactory.PopulateEmptyStrings = true; at the start of each method
        private readonly TestEntityFactory testEntityFactory = new TestEntityFactory();

        /// <summary>
        /// Test adding a unit to the database
        /// </summary>
        [TestMethod]
        public void AddUnit()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a unit. ID provided is 0, which will indicates creation of new helpdesk.
            List<string> topics = new List<string>(new string[] { "Layouts", "Lifecycle"});
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, "", "", false, topics);

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                int confirmId = unitData.Response.UnitID;
                var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == confirmId);
                Assert.IsNotNull(unit);
                Assert.IsTrue
                (
                    unitData.Request.IsDeleted == unit.IsDeleted
                    && unitData.Request.Name == unit.Name
                    && unitData.Request.Code == unit.Code
                    && unitData.Request.Topics.Count + 1 == unit.Topic.Count
                    && unit.Topic.Count > 0
                );
            }
        }

        /// <summary>
        /// Test adding a unit to the database with a topic name that
        /// is too large is handled correctly
        /// </summary>
        [TestMethod]
        public void AddUnitTopicNameTooLong()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a unit. ID provided is 0, which will indicates creation of new helpdesk.
            List<string> topics = new List<string>(new string[] { AlphaNumericStringGenerator.GetString(51),});
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, "", "", false, topics);

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);
        }

        /// <summary>
        /// Test adding a unit to the database with no helpdesk id to reference.
        /// </summary>
        [TestMethod]
        public void AddUnitNoHelpdesk()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Create unit with null helpdesk id.
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, null);
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);
        }

        /// <summary>
        /// Test adding a unit to the database with no name
        /// </summary>
        [TestMethod]
        public void AddUnitNoName()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Create unit with null name.
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, 1, null);
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);

            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = false;

            // Create unit with empty name.
            unitData = testEntityFactory.AddUpdateUnit(0, 1, "");
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);
        }

        /// <summary>
        /// Test adding a unit to the database with no code
        /// </summary>
        [TestMethod]
        public void AddUnitNoCode()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Create unit with empty string for code.
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, 1, "", null);
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);
        }

        /// <summary>
        /// Test adding a unit to the database with wrong name length
        /// </summary>
        [TestMethod]
        public void AddUnitNameWrongLength()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Create unit with empty string for code.
            string name = AlphaNumericStringGenerator.GetString(100);
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, 1, name);
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);
        }

        /// <summary>
        /// Test adding a unit to the database with wrong code length
        /// </summary>
        [TestMethod]
        public void AddUnitCodeWrongLength()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Create unit with empty string for code.
            string code = AlphaNumericStringGenerator.GetString(20);
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, 1, "", code);
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);
        }

        /// <summary>
        /// Test adding a unit with a duplicate name to the database
        /// </summary>
        [TestMethod]
        public void AddUnitDupName()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a unit.
            List<string> topics = new List<string>(new string[] { "Layouts", "Lifecycle" });
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, "", "", false, topics);

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            // Attempt to create unit with duplicate name.
            unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, unitData.Request.Name, "", false, topics);

            // Check that duplicate name yields bad request.
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);
        }

        /// <summary>
        /// Test adding a unit with a duplicate name to the database
        /// </summary>
        [TestMethod]
        public void AddUnitDupCode()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a unit.
            List<string> topics = new List<string>(new string[] { "Layouts", "Lifecycle" });
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, "", "", false, topics);

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            // Attempt to create unit with duplicate name.
            unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, "", unitData.Request.Code, false, topics);

            // Check that duplicate name yields bad request.
            Assert.AreEqual(HttpStatusCode.BadRequest, unitData.Response.Status);
        }

        /// <summary>
        /// Test updating a unit in a database
        /// </summary>
        [TestMethod]
        public void UpdateUnit()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a unit. ID provided is 0, which will indicates creation of new helpdesk.
            List<string> topics = new List<string>(new string[] { "Layouts", "Lifecycle"});
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, "", "", false, topics);

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                int confirmId = unitData.Response.UnitID;
                var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == confirmId);
                Assert.IsNotNull(unit);
                Assert.IsTrue
                (
                    unitData.Request.IsDeleted == unit.IsDeleted
                    && unitData.Request.Name == unit.Name
                    && unitData.Request.Code == unit.Code
                    && unitData.Request.Topics.Count + 1 == unit.Topic.Count
                    && unit.Topic.Count > 0
                );
            }

            // Update unit using id of previously created unit. Add "Externalisation" as topic. 
            topics = new List<string>(new string[] { "Layouts", "Externalisation" });
            TestDataUnit unitDataUpdate = testEntityFactory.AddUpdateUnit(unitData.Response.UnitID, helpdeskData.Response.HelpdeskID,
                unitData.Request.Name, unitData.Request.Code, unitData.Request.IsDeleted, topics);

            // Check that unit was updated successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitDataUpdate.Response.Status);
            Assert.IsTrue(unitDataUpdate.Response.UnitID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == unitData.Response.UnitID);
                Assert.IsNotNull(unit);
                Assert.IsTrue
                (
                    unitData.Request.IsDeleted == unit.IsDeleted
                    && unitData.Request.Name == unit.Name
                );
                Assert.IsFalse(unit.Topic.FirstOrDefault(t => t.Name == "Layouts").IsDeleted);
                Assert.IsFalse(unit.Topic.FirstOrDefault(t => t.Name == "Externalisation").IsDeleted);
                Assert.IsTrue(unit.Topic.FirstOrDefault(t => t.Name == "Lifecycle").IsDeleted);
            }
        }

        /// <summary>
        /// Test updating a unit in a database
        /// </summary>
        [TestMethod]
        public void UpdateUnitNotFound()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create unit with non-existing id.
            int maxInt = 2147483647;
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(maxInt, helpdeskData.Response.HelpdeskID);
            Assert.AreEqual(HttpStatusCode.NotFound, unitData.Response.Status);
        }

        /// <summary>
        /// Test retrieving a unit from the database using an id.
        /// Retrieving the test unit id 1, named "Test Unit".
        /// </summary>
        [TestMethod]
        public void GetUnit()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a unit. ID provided is 0, which will indicates creation of new helpdesk.
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID);

            Topic topic = new Topic()
            {
                Name = AlphaNumericStringGenerator.GetString(10),
                IsDeleted = false,
                UnitId = unitData.Response.UnitID
            };
            Topic deletedTopic = new Topic()
            {
                Name = AlphaNumericStringGenerator.GetString(10),
                IsDeleted = true,
                UnitId = unitData.Response.UnitID
            };

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                context.Topic.Add(topic);
                context.Topic.Add(deletedTopic);
                context.SaveChanges();
            }

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            // Get the unit that was just created.
            UnitsFacade unitsFacade = new UnitsFacade();
            GetUnitResponse getUnitResponse = unitsFacade.GetUnit(unitData.Response.UnitID);

            // Check that unit response is okay and that names match.
            Assert.AreEqual(HttpStatusCode.OK, getUnitResponse.Status);
            Assert.AreEqual(unitData.Request.Name, getUnitResponse.Unit.Name);
            Assert.IsTrue(getUnitResponse.Unit.Topics.Count == 2);
            Assert.AreEqual(topic.Name, getUnitResponse.Unit.Topics[1].Name);
        }

        /// <summary>
        /// Used to ensure getting all units in a specific helpdesk works
        /// </summary>
        [TestMethod]
        public void GetAllUnitsByHelpdeskID()
        {
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a deleted unit. ID provided is 0, which will indicates creation of new helpdesk.
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, isDeleted: true);
            TestDataUnit unitData2 = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID);
            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            // Get all units that were just created.
            UnitsFacade unitsFacade = new UnitsFacade();
            GetUnitsByHelpdeskIDResponse getUnitsByHelpdeskIDResponse = unitsFacade.GetUnitsByHelpdeskID(helpdeskData.Response.HelpdeskID, false);

            Assert.AreEqual(HttpStatusCode.OK, getUnitsByHelpdeskIDResponse.Status);
            Assert.AreEqual(unitData.Request.Name, getUnitsByHelpdeskIDResponse.Units[0].Name);
            Assert.AreEqual(unitData2.Request.Name, getUnitsByHelpdeskIDResponse.Units[1].Name);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdeskUnits = context.Helpdeskunit.ToList();

                Assert.IsNotNull(helpdeskUnits);
            }
        }

        /// <summary>
        /// Test getting all units with helpdesk ID "1"
        /// </summary>
        [TestMethod]
        public void GetActiveUnitsByHelpdeskID()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            // Add test helpdesk.
            TestDataHelpdesk helpdeskData = testEntityFactory.AddHelpdesk();

            // Check that helpdesk was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, helpdeskData.Response.Status);
            Assert.IsTrue(helpdeskData.Response.HelpdeskID > 0);

            // Create a unit. ID provided is 0, which will indicates creation of new unit.
            TestDataUnit unitData = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID);

            // Create a deleted unit. ID provided is 0, which will indicates creation of new unit.
            TestDataUnit unitData2 = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID, isDeleted: true);


            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitData.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            // Create another unit.
            TestDataUnit unitDataAgain = testEntityFactory.AddUpdateUnit(0, helpdeskData.Response.HelpdeskID);

            // Check that unit was created successfully.
            Assert.AreEqual(HttpStatusCode.OK, unitDataAgain.Response.Status);
            Assert.IsTrue(unitData.Response.UnitID > 0);

            // Get all units that were just created.
            UnitsFacade unitsFacade = new UnitsFacade();
            GetUnitsByHelpdeskIDResponse getUnitsByHelpdeskIDResponse = unitsFacade.GetUnitsByHelpdeskID(helpdeskData.Response.HelpdeskID, true);

            Assert.AreEqual(HttpStatusCode.OK, getUnitsByHelpdeskIDResponse.Status);
            Assert.AreEqual(unitData.Request.Name, getUnitsByHelpdeskIDResponse.Units[0].Name);
            Assert.AreEqual(unitDataAgain.Request.Name, getUnitsByHelpdeskIDResponse.Units[1].Name);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdeskUnits = context.Helpdeskunit.ToList();

                Assert.IsNotNull(helpdeskUnits);
            }
        }

        /// <summary>
        /// Test getting all units from a helpdesk that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void GetUnitsByHelpdeskIDNotFound()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            UnitsFacade unitsFacade = new UnitsFacade();
            int maxInt = 2147483647;
            GetUnitsByHelpdeskIDResponse getUnitsByHelpdeskIDResponse = unitsFacade.GetUnitsByHelpdeskID(maxInt, false);

            Assert.AreEqual(HttpStatusCode.NotFound, getUnitsByHelpdeskIDResponse.Status);
        }

        /// <summary>
        /// Test successful deletion of unit from the database.
        /// </summary>
        [TestMethod]
        public void DeleteUnit()
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

            // Test get, delete, get.
            UnitsFacade unitsFacade = new UnitsFacade();

            // Get the unit that was just created.
            GetUnitResponse getUnitResponse = unitsFacade.GetUnit(unitData.Response.UnitID);
            Assert.AreEqual(HttpStatusCode.OK, getUnitResponse.Status);

            // Delete the unit that was just created.
            DeleteUnitResponse deleteUnitResponse = unitsFacade.DeleteUnit(unitData.Response.UnitID);
            Assert.AreEqual(HttpStatusCode.OK, deleteUnitResponse.Status);

            // Try getting the unit that was just deleted. Should be NotFound.
            //Will update unit test when get unit method is implemented that excludes units
            //with IsDeleted = true
            getUnitResponse = unitsFacade.GetUnit(unitData.Response.UnitID);
            Assert.IsTrue(getUnitResponse.Unit.IsDeleted);
        }

        /// <summary>
        /// Test deleting a unit that doesn't exist is handled properly
        /// </summary>
        [TestMethod]
        public void DeleteUnitNotFound()
        {
            // Fill empty string parameters "" with auto-generated string.
            testEntityFactory.PopulateEmptyStrings = true;

            UnitsFacade unitsFacade = new UnitsFacade();
            int maxInt = 2147483647;
            DeleteUnitResponse deleteUnitResponse = unitsFacade.DeleteUnit(maxInt);

            Assert.AreEqual(HttpStatusCode.NotFound, deleteUnitResponse.Status);
        }

    }
}
