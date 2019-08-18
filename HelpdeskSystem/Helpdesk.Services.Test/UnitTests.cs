using System;
using System.Net;
using Helpdesk.Common.Responses.Units;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helpdesk.Services.Test
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void GetUnit()
        {
            UnitsFacade unitsFacade = new UnitsFacade();
            int unitId = 1;

            GetUnitResponse getUnitResponse = unitsFacade.GetUnit(unitId);

            Assert.AreEqual(HttpStatusCode.OK, getUnitResponse.Status);
            Assert.AreEqual("Test Unit" , getUnitResponse.Unit.Name);
        }
        
    }
}
