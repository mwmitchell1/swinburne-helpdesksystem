using System;
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
        /// <summary>
        /// Ensures that adding a helpdesk works
        /// </summary>
        [TestMethod]
        public void AddHelpdesk()
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
        /// Tests adding a timespan to the database with a valid request.
        /// </summary>
        [TestMethod]
        public void AddTimespan()
        {
            HelpdeskFacade helpdeskFacade = new HelpdeskFacade();

            AddTimeSpanRequest addTimeSpanRequest = new AddTimeSpanRequest();
            addTimeSpanRequest.HelpdeskId = 1;
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
            addTimeSpanRequest.HelpdeskId = 1;
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
            addTimeSpanRequest.HelpdeskId = 1;
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