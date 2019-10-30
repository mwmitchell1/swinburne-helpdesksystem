using System;
using System.Collections.Generic;
using System.Net;
using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Extensions;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Topics;
using Helpdesk.DataLayer;
using Microsoft.Extensions.Logging;
using NLog;

namespace Helpdesk.Services
{
    /// <summary>
    /// This class is used to handle the business logic of topics
    /// </summary>
    public class TopicsFacade : ILoginClass
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        public TopicsFacade()
        {
            _appSettings = new AppSettings();
        }

        /// <summary>
        /// This method is used to get all topics of a specific unit
        /// </summary>
        /// <param name="id">ID of the unit to get topics from</param>
        /// <returns>Response which indicates success or failure</returns>
        public GetTopicsByUnitIDResponse GetTopicsByUnitID(int id)
        {
            var response = new GetTopicsByUnitIDResponse();

            try
            {
                var dataLayer = new TopicsDataLayer();
                List<TopicDTO> topics = dataLayer.GetTopicsByUnitID(id);

                if (topics.Count == 0)
                {
                    throw new NotFoundException("No topics found under unit " + id);
                }

                response.Topics = topics;
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Error(ex, "No unit found found matching id " + id);
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "No topics found!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to get topics!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get topics!"));
            }
            return response;
        }
    }
}
