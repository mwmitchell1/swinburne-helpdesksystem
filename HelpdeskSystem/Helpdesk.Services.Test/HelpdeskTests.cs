using System;
using System.Net;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Common.Responses.Helpdesk;
using Helpdesk.Common.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Helpdesk.Services.Test
{
    [TestClass]
    public class HelpdeskTests
    {
        /// <summary>
        /// Tests adding a timespan to the database with a valid request.
        /// </summary>
        [TestMethod]
        public void AddTimespan()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest();
            addTimeSpanRequest.Name = AlphaNumericStringGenerator.GetString(10);
            DateTime startDate = DateTime.Today;
            DateTime endDate = new DateTime(startDate.Year + 1, startDate.Month, startDate.Day, 0, 0, 0);
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

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest();
            addTimeSpanRequest.Name = AlphaNumericStringGenerator.GetString(10);
            DateTime startDate = DateTime.Today;
            DateTime endDate = new DateTime(startDate.Year - 1, startDate.Month, startDate.Day, 0, 0, 0);
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

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
            addTimeSpanRequest.Name = AlphaNumericStringGenerator.GetString(10);
            DateTime startDate = new DateTime(2018, 1, 1, 0, 0, 0);
            DateTime endDate = DateTime.Today;
            addTimeSpanRequest.StartDate = startDate;
            addTimeSpanRequest.EndDate = endDate;

            AddTimeSpanResponse addTimeSpanResponse = helpdeskFacade.AddTimeSpan(addTimeSpanRequest);

            Assert.AreEqual(HttpStatusCode.BadRequest, addTimeSpanResponse.Status);
        }
    }
}