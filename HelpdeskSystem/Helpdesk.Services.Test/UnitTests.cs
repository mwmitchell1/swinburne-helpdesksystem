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
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// Test adding a unit to the database
        /// </summary>
        [TestMethod]
        public void AddUnit()
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

            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = addHelpdeskResponse.HelpdeskID,
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
                    && request.Topics.Count == unit.Topic.Count
                    && unit.Topic.Count > 0
                );
            }
        }

        /// <summary>
        /// Test updating a unit in a database
        /// </summary>
        [TestMethod]
        public void UpdateUnit()
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

            string name = AlphaNumericStringGenerator.GetString(5);
            string code = AlphaNumericStringGenerator.GetString(8);

            UnitsFacade unitsFacade = new UnitsFacade();
            var request = new AddUpdateUnitRequest()
            {
                HelpdeskID = addHelpdeskResponse.HelpdeskID,
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

            request = new AddUpdateUnitRequest
            {
                UnitID = addUpdateUnitResponse.UnitID,
                HelpdeskID = addHelpdeskResponse.HelpdeskID,
                Code = code,
                IsDeleted = false,
                Name = name,
            };

            request.Topics.Add("Layouts");
            request.Topics.Add("Externalisation");

            addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(request);

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
                );

                Assert.IsFalse(unit.Topic.FirstOrDefault(t => t.Name == "Layouts").IsDeleted);
                Assert.IsFalse(unit.Topic.FirstOrDefault(t => t.Name == "Externalisation").IsDeleted);
                Assert.IsTrue(unit.Topic.FirstOrDefault(t => t.Name == "Lifecycle").IsDeleted);
            }
        }

        /// <summary>
        /// Test retrieving a unit from the database using an id.
        /// Retrieving the test unit id 1, named "Test Unit".
        /// </summary>
        [TestMethod]
        public void GetUnit()
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

            AddUpdateUnitRequest addUnitRequest = new AddUpdateUnitRequest
            {
                HelpdeskID = addHelpdeskResponse.HelpdeskID,
                Name = AlphaNumericStringGenerator.GetString(10),
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false
            };

            UnitsFacade unitsFacade = new UnitsFacade();
            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(addUnitRequest);

            Assert.AreEqual(HttpStatusCode.OK, addUpdateUnitResponse.Status);

            GetUnitResponse getUnitResponse = unitsFacade.GetUnit(addUpdateUnitResponse.UnitID);

            Assert.AreEqual(HttpStatusCode.OK, getUnitResponse.Status);
            Assert.AreEqual(addUnitRequest.Name, getUnitResponse.Unit.Name);
        }

        /// <summary>
        /// Test getting all units with helpdesk ID "1"
        /// </summary>
        [TestMethod]
        public void GetUnitsByHelpdeskID()
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

            AddUpdateUnitRequest addUnitRequest = new AddUpdateUnitRequest
            {
                HelpdeskID = addHelpdeskResponse.HelpdeskID,
                Name = AlphaNumericStringGenerator.GetString(10),
                Code = AlphaNumericStringGenerator.GetString(8),
                IsDeleted = false
            };

            UnitsFacade unitsFacade = new UnitsFacade();
            AddUpdateUnitResponse addUpdateUnitResponse = unitsFacade.AddOrUpdateUnit(addUnitRequest);

            Assert.AreEqual(HttpStatusCode.OK, addUpdateUnitResponse.Status);

            GetUnitResponse getUnitResponse = unitsFacade.GetUnit(addUnitRequest.HelpdeskID);

            Assert.AreEqual(HttpStatusCode.OK, getUnitResponse.Status);

            GetUnitsByHelpdeskIDResponse getUnitsByHelpdeskIDResponse = unitsFacade.GetUnitsByHelpdeskID(addHelpdeskResponse.HelpdeskID);

            Assert.AreEqual(HttpStatusCode.OK, getUnitsByHelpdeskIDResponse.Status);
            Assert.AreEqual(addUnitRequest.Name, getUnitsByHelpdeskIDResponse.Units[0].Name);

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

            GetUnitsByHelpdeskIDResponse getUnitsByHelpdeskIDResponse = unitsFacade.GetUnitsByHelpdeskID(-1);

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

            DeleteUnitResponse deleteUnitResponse = unitsFacade.DeleteUnit(-1);

            Assert.AreEqual(HttpStatusCode.NotFound, deleteUnitResponse.Status);
        }

    }
}
