using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Extensions;
using Helpdesk.Common.Requests.Helpdesk;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Helpdesk;
using Helpdesk.DataLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace Helpdesk.Services
{
    /// <summary>
    /// Used to handle business logic related to helpdesks and their report timespans
    /// </summary>
    public class HelpdeskFacade
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        public HelpdeskFacade()
        {
            _appSettings = new AppSettings();
        }

        /// <summary>
        /// This method is to handle adding helpdesk business logic
        /// </summary>
        /// <param name="request">This is the request with the info to add the helpdesk</param>
        /// <returns>Returns a response with the id and indications of success</returns>
        public AddHelpdeskResponse AddHelpdesk(AddHelpdeskRequest request)
        {
            var response = new AddHelpdeskResponse();

            try
            {
                response = (AddHelpdeskResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                var dataLayer = new HelpdeskDataLayer();
                int? helpdeskId = dataLayer.AddHelpdesk(request);
                if (helpdeskId.HasValue)
                {
                    response.HelpdeskID = helpdeskId.Value;
                    response.Status = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add helpdesk"));
                s_logger.Error(ex, "Unable to add helpdesk.");
            }

            return response;
        }

        /// <summary>
        /// This method is to handle updating helpdesk business logic
        /// </summary>
        /// <param name="id">The id of the helpdesk to be updated</param>
        /// <param name="request">This is the request with the info to update the helpdesk</param>
        /// <returns>Returns a response which indicate the result</returns>
        public UpdateHelpdeskResponse UpdateHelpdesk(int id, UpdateHelpdeskRequest request)
        {
            var response = new UpdateHelpdeskResponse();

            try
            {
                response = (UpdateHelpdeskResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                var dataLayer = new HelpdeskDataLayer();
                bool result = dataLayer.UpdateHelpdesk(id, request);

                if (result)
                    response.Status = HttpStatusCode.OK;
                else
                {
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Unable to update helpdesk."));
                }
            }
            catch (NotFoundException ex)
            {
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to update to find helpdesk"));
                s_logger.Error(ex, "Unable to find helpdesk.");
            }
            catch (Exception ex)
            {
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to update helpdesk"));
                s_logger.Error(ex, "Unable to update helpdesk.");
            }

            return response;
        }

        public GetTimeSpansResponse GetTimeSpans()
        {
            throw new NotImplementedException();
        }

        public GetTimeSpanResponse GetTimeSpan(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is responsible for adding a new timespan.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public AddTimeSpanResponse AddTimeSpan(AddTimeSpanRequest request)
        {
            s_logger.Info("Adding timespan...");

            AddTimeSpanResponse response = new AddTimeSpanResponse();

            try
            {
                response = (AddTimeSpanResponse)request.CheckValidation(response);
                if (response.Status == HttpStatusCode.BadRequest)
                {
                    return response;
                }

                //TODO Need a method to check timespan name in request against names in the database.
                // Assuming timespans are unique?
                /*
                var dataLayer = new HelpdeskDataLayer();

                if (dataLayer.GetTimeSpanByName(request.Name) != null)
                {
                    throw new Exception("Unable to add timespan! Timespan already exists!");
                }
                */

                var dataLayer = new HelpdeskDataLayer();

                int? result = dataLayer.AddTimeSpan(request);

                if (result == null)
                {
                    throw new Exception("Unable to add timespan!");
                }

                response.SpanId = (int)result;
                response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to add timespan!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add timespan!"));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for updating a specific timespan's information
        /// </summary>
        /// <param name="id">The SpanId of the timespan to be updated</param>
        /// <param name="request">The timespan's new information</param>
        /// <returns>The response that indicates if the operation was a success</returns>
        public UpdateTimeSpanResponse UpdateTimeSpan(int id, UpdateTimeSpanRequest request)
        {
            s_logger.Info("Updating timespan...");

            UpdateTimeSpanResponse response = new UpdateTimeSpanResponse();

            try
            {
                response = (UpdateTimeSpanResponse)request.CheckValidation(response);
                if (response.Status == HttpStatusCode.BadRequest)
                {
                    return response;
                }

                var dataLayer = new HelpdeskDataLayer();

                bool result = dataLayer.UpdateTimeSpan(id, request);

                if (result == false)
                    throw new Exception("Unable to update timespan!");

                response.result = result;
                response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to update timespan!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to update timespan!"));
            }
            return response;
        }

        public DeleteTimeSpanResponse DeleteTimeSpan(int id)
        {
            throw new NotImplementedException();
        }
    }
}
