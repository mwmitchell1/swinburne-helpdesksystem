using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Extensions;
using Helpdesk.Common.Requests;
using Helpdesk.Common.Requests.CheckIn;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.CheckIn;
using Helpdesk.DataLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Helpdesk.Services
{
    /// <summary>
    /// This class is used to handle the business logic of checking in
    /// </summary>
    public class CheckInFacade
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This method is used to check into the database
        /// </summary>
        /// <param name="request">The request containing the specified UnitID</param>
        /// <returns>A response indicating success or failure</returns>
        public CheckInResponse CheckIn(CheckInRequest request)
        {
            CheckInResponse response = new CheckInResponse();

            try
            {
                response = (CheckInResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                CheckInDataLayer dataLayer = new CheckInDataLayer();
                int checkInID = dataLayer.CheckIn(request);

                response.CheckInID = checkInID;
                response.Status = HttpStatusCode.OK;
            }
            catch(NotFoundException ex)
            {
                s_logger.Warn(ex, "Unable to check in");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to check in"));
            }
            catch(Exception ex)
            {
                s_logger.Error(ex, "Unable to check in");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to check in"));
            }
            return response;
        }

        /// <summary>
        /// This method is used to check a check in item out of the database
        /// </summary>
        /// <param name="id">Specified CheckInID</param>
        /// <returns>A response indicating success or failure</returns>
        public CheckOutResponse CheckOut(int id)
        {
            CheckOutResponse response = new CheckOutResponse();

            try
            {
               CheckInDataLayer dataLayer = new CheckInDataLayer();

                bool result = dataLayer.CheckOut(id);

                if (result == false)
                    throw new NotFoundException("Unable to find check in item!");

                response.Result = result;
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Warn(ex, "Unable to find check out item");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to find check out item"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to check out");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to check out"));
            }
            return response;
        }
    }
}
