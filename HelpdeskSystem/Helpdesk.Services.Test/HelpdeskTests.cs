using System;
using System.Net;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Common.Responses.Helpdesk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helpdesk.Services.Test
{
    [TestClass]
    public class HelpdeskTests
    {
        /// <summary>
        /// Tests adding a timespan to the database using a valid request.
        /// </summary>
        [TestMethod]
        public void AddTimespan()
        {
            // NOTE: This test will fail if the user already exists!
            // Need to decide how to handle tests that conflict with previous tests.
            // WR: You can use a auto gen alpha string function, there is one on stack overflow
            // if you Google it.
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest();
            addTimeSpanRequest.Name = "Test Period";
            DateTime startDate = DateTime.Today;
            DateTime endDate = new DateTime(startDate.Year + 1, startDate.Month, startDate.Day, 0, 0, 0);
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.OK, addTimeSpanResponse.Status);
        }
    }
}