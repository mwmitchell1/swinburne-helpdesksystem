using System;
using System.Net;
using System.Linq;
using Helpdesk.Common.Responses.Units;
using Helpdesk.Data.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helpdesk.Services.Test
{
    [TestClass]
    public class UnitTests
    {
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
            Assert.AreEqual("Test Unit" , getUnitResponse.Unit.Name);
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

    }
}
