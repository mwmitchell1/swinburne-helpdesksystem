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

namespace Helpdesk.Services.Test
{
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// Test adding a unit to the database
        /// </summary>
        [TestMethod]
        public void AddUnit()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = 1,
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(5),
            };
            request.Topics.Add("Layouts");
            request.Topics.Add("Lifecycle");

            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.OK, addUpdateUnitResponse.Status);
            Assert.IsTrue(addUpdateUnitResponse.UnitID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == addUpdateUnitResponse.UnitID);
                Assert.IsNotNull(unit);
                Assert.IsTrue
                (
                    request.IsDeleted == unit.IsDeleted
                    && request.Name == unit.Name
                    && request.Code == unit.Code
                    && request.Topics.Count == unit.Topic.Count
                    && unit.Topic.Count > 0
                );
            }
        }

        /// <summary>
        /// Test adding a unit to the database with no code
        /// </summary>
        [TestMethod]
        public void AddUnitNoCode()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = 1,
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(5),
            };
            request.Topics.Add("Layouts");
            request.Topics.Add("Lifecycle");

            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUpdateUnitResponse.Status);
        }

        /// <summary>
        /// Test adding a unit to the database with no helpdesk
        /// </summary>
        [TestMethod]
        public void AddUnitNoHelpdesk()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(5),
            };
            request.Topics.Add("Layouts");
            request.Topics.Add("Lifecycle");

            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUpdateUnitResponse.Status);
        }

        /// <summary>
        /// Test adding a unit to the database with no name
        /// </summary>
        [TestMethod]
        public void AddUnitNoName()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = 1,
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
            };
            request.Topics.Add("Layouts");
            request.Topics.Add("Lifecycle");

            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUpdateUnitResponse.Status);
        }

        /// <summary>
        /// Test adding a unit to the database with wrong code length
        /// </summary>
        [TestMethod]
        public void AddUnitCodeWrongLength()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = 1,
                Code = AlphaNumericStringGenerator.GetString(10),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(5),
            };
            request.Topics.Add("Layouts");
            request.Topics.Add("Lifecycle");

            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUpdateUnitResponse.Status);
        }

        /// <summary>
        /// Test adding a unit with a duplicate name to the database
        /// </summary>
        [TestMethod]
        public void AddUnitDupName()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = 1,
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(5),
            };
            request.Topics.Add("Layouts");
            request.Topics.Add("Lifecycle");

            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.OK, addUpdateUnitResponse.Status);
            Assert.IsTrue(addUpdateUnitResponse.UnitID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == addUpdateUnitResponse.UnitID);
                Assert.IsNotNull(unit);
                Assert.IsTrue
                (
                    request.IsDeleted == unit.IsDeleted
                    && request.Name == unit.Name
                    && request.Code == unit.Code
                    && request.Topics.Count == unit.Topic.Count
                    && unit.Topic.Count > 0
                );
            }

            request.Code = AlphaNumericStringGenerator.GetString(8);

            addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUpdateUnitResponse.Status);
        }

        /// <summary>
        /// Test adding a unit with a duplicate name to the database
        /// </summary>
        [TestMethod]
        public void AddUnitDupCode()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = 1,
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false,
                Name = AlphaNumericStringGenerator.GetString(5),
            };
            request.Topics.Add("Layouts");
            request.Topics.Add("Lifecycle");

            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.OK, addUpdateUnitResponse.Status);
            Assert.IsTrue(addUpdateUnitResponse.UnitID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == addUpdateUnitResponse.UnitID);
                Assert.IsNotNull(unit);
                Assert.IsTrue
                (
                    request.IsDeleted == unit.IsDeleted
                    && request.Name == unit.Name
                    && request.Code == unit.Code
                    && request.Topics.Count == unit.Topic.Count
                    && unit.Topic.Count > 0
                );
            }

            request.Name = AlphaNumericStringGenerator.GetString(10);

            addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.BadRequest, addUpdateUnitResponse.Status);
        }

        /// <summary>
        /// Test updating a unit in a database
        /// </summary>
        [TestMethod]
        public void UpdateUnit()
        {
            string name = AlphaNumericStringGenerator.GetString(5);
            string code = AlphaNumericStringGenerator.GetString(8);

            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = 1,
                Code = code,
                IsDeleted = false,
                Name = name,
            };
            request.Topics.Add("Layouts");
            request.Topics.Add("Lifecycle");

            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            Assert.AreEqual(HttpStatusCode.OK, addUpdateUnitResponse.Status);
            Assert.IsTrue(addUpdateUnitResponse.UnitID > 0);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == addUpdateUnitResponse.UnitID);
                Assert.IsNotNull(unit);
                Assert.IsTrue
                (
                    request.IsDeleted == unit.IsDeleted
                    && request.Name == unit.Name
                    && request.Topics.Count == unit.Topic.Count
                    && unit.Topic.Count > 0
                );
            }

            request = new AddUpdateUnitRequest()
            {
                UnitID = addUpdateUnitResponse.UnitID,
                HelpdeskID = 1,
                Code = code,
                IsDeleted = false,
                Name = name,
            };

            request.Topics.Add("Layouts");
            request.Topics.Add("Externalisation");

            addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unit = context.Unit.Include("Topic").FirstOrDefault(u => u.UnitId == addUpdateUnitResponse.UnitID);
                Assert.IsNotNull(unit);
                Assert.IsTrue
                (
                    request.IsDeleted == unit.IsDeleted
                    && request.Name == unit.Name
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
            string name = AlphaNumericStringGenerator.GetString(5);
            string code = AlphaNumericStringGenerator.GetString(8);

            UnitsFacade unitsFacade = new UnitsFacade();

            AddUpdateUnitRequest request = new AddUpdateUnitRequest()
            {
                UnitID = 100000000,
                HelpdeskID = 1,
                Code = code,
                IsDeleted = false,
                Name = name,
            };

            request.Topics.Add("Layouts");
            request.Topics.Add("Externalisation");

            var addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);
            Assert.AreEqual(HttpStatusCode.NotFound, addUpdateUnitResponse.Status);
        }

        /// <summary>
        /// Test retrieving a unit from the database using an id.
        /// Retrieving the test unit id 1, named "Test Unit".
        /// </summary>
        [TestMethod]
        public void GetUnit()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            int unitId = 1;

            GetUnitResponse getUnitResponse = unitsFacade.GetUnit(unitId);

            Assert.AreEqual(HttpStatusCode.OK, getUnitResponse.Status);
            Assert.AreEqual("Test Unit", getUnitResponse.Unit.Name);
        }

        /// <summary>
        /// Test getting all units with helpdesk ID "1"
        /// </summary>
        [TestMethod]
        public void GetUnitsByHelpdeskID()
        {
            UnitsFacade unitsFacade = new UnitsFacade();

            GetUnitsByHelpdeskIDResponse getUnitsByHelpdeskIDResponse = unitsFacade.GetUnitsByHelpdeskID(1);

            Assert.AreEqual(HttpStatusCode.OK, getUnitsByHelpdeskIDResponse.Status);
            Assert.AreEqual("Test Unit", getUnitsByHelpdeskIDResponse.Units[0].Name);

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var helpdeskUnits = context.Helpdeskunit.ToList();

                Assert.IsNotNull(helpdeskUnits);
            }
        }

        /// <summary>
        /// Test getting all units from a helpdesk that doesn't exist is handeled properly
        /// </summary>
        [TestMethod]
        public void GetUnitsByHelpdeskIDNotFound()
        {
            UnitsFacade unitsFacade = new UnitsFacade();

            GetUnitsByHelpdeskIDResponse getUnitsByHelpdeskIDResponse = unitsFacade.GetUnitsByHelpdeskID(2);

            Assert.AreEqual(HttpStatusCode.NotFound, getUnitsByHelpdeskIDResponse.Status);
        }

        //Test deleting a unit to be implemented when adding a unit is implemented

        /// <summary>
        /// Test deleting a unit that doesn't exist is handeled properly
        /// </summary>
        [TestMethod]
        public void DeleteUnitNotFound()
        {
            UnitsFacade unitsFacade = new UnitsFacade();

            DeleteUnitResponse deleteUnitResponse = unitsFacade.DeleteUnit(0);

            Assert.AreEqual(HttpStatusCode.NotFound, deleteUnitResponse.Status);
        }

    }
}
