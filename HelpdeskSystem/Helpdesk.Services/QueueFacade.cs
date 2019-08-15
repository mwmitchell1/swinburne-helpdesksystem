using Helpdesk.Common;
using Helpdesk.Common.Requests.Queue;
using Helpdesk.Common.Requests.Students;
using Helpdesk.Common.Responses;
using Helpdesk.Common.Responses.Queue;
using Helpdesk.DataLayer;
using NLog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Helpdesk.Services
{
    /// <summary>
    /// This class is used to handle the business logic of queues
    /// </summary>
    public class QueueFacade
    {
        private static Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly AppSettings _appSettings;

        /// <summary>
        /// This method is used to add an item to a queue
        /// </summary>
        /// <param name="request">The information of the queue item</param>
        /// <returns></returns>
        public AddToQueueResponse AddToQueue(AddToQueueRequest request)
        {
            AddToQueueResponse response = new AddToQueueResponse();

            try
            {
                response = (AddToQueueResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                StudentFacade studentFacade = new StudentFacade();

                if (!request.StudentID.HasValue)
                {
                    var addResponse = studentFacade.AddStudentNickname(new AddStudentRequest()
                    {
                        Nickname = request.Nickname,
                        SID = request.SID
                    });

                    if (addResponse.Status != HttpStatusCode.OK)
                    {
                        response.Status = addResponse.Status;
                        response.StatusMessages = addResponse.StatusMessages;
                        return response;
                    }

                    request.StudentID = addResponse.StudentID;
                }

                QueueDataLayer dataLayer = new QueueDataLayer();
                int itemId = dataLayer.AddToQueue(request);

                response.ItemId = itemId;
                response.Status = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to add queue item");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add queue item"));
            }
            return response;
        }
    }
}
