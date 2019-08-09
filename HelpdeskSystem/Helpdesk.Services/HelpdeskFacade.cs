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
    public class HelpdeskFacade
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        public HelpdeskFacade()
        {
            _appSettings = new AppSettings();
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

                var dataLayer = new HelpdeskDataLayer();

                int? result = dataLayer.AddTimeSpan(request);

                if (result == null)
                {
                    throw new Exception("Unable to add timespan!");
                }

                response.SpanId = (int)result;
                response.Status = HttpStatusCode.OK;
            }
            catch(Exception ex)
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
