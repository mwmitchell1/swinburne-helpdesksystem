using Helpdesk.Common;
using Helpdesk.Common.DTOs;
using Helpdesk.Common.Extensions;
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
            catch (NotFoundException ex)
            {
                s_logger.Warn(ex, "Unable to add queue item");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to add queue item"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to add queue item");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to add queue item"));
            }
            return response;
        }

        public UpdateQueueItemResponse UpdateQueueItem(int id, UpdateQueueItemRequest request)
        {
            UpdateQueueItemResponse response = new UpdateQueueItemResponse();

            try
            {
                response = (UpdateQueueItemResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                    return response;

                QueueDataLayer dataLayer = new QueueDataLayer();
                response.Result = dataLayer.UpdateQueueItem(id, request);
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Warn(ex, "Unable to update queue item");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to update queue item"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to update queue item");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to update queue item"));
            }
            return response;
        }

        /// <summary>
        /// This method updates a queue items status by either entering a helped time or removed time.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public UpdateQueueItemStatusResponse UpdateQueueItemStatus(int id, UpdateQueueItemStatusRequest request)
        {
            UpdateQueueItemStatusResponse response = new UpdateQueueItemStatusResponse();

            try
            {
                response = (UpdateQueueItemStatusResponse)request.CheckValidation(response);

                if (response.Status == HttpStatusCode.BadRequest)
                {
                    return response;
                }

                QueueDataLayer dataLayer = new QueueDataLayer();
                bool result = dataLayer.UpdateQueueItemStatus(id, request);

                if (result)
                {
                    response.Status = HttpStatusCode.OK;
                    response.Result = true;
                }
                else
                {
                    response.Status = HttpStatusCode.BadRequest;
                    response.StatusMessages.Add(new StatusMessage(HttpStatusCode.BadRequest, "Unable to update queue item status."));
                }
            }
            catch (NotFoundException ex)
            {
                s_logger.Warn(ex, "Unable to update queue item status");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "Unable to update queue item status"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to update queue item status.");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to update queue item status."));
            }
            return response;
        }

        /// <summary>
        /// This method is responsible for retrieving all queue items from the helpdesk system
        /// </summary>
        /// <returns>The response that indicates if the operation was a success,
        /// and the list of queue items</returns>
        public GetQueueItemsByHelpdeskIDResponse GetQueueItemsByHelpdeskID(int id)
        {
            s_logger.Info("Getting queue items by helpdesk id...");

            GetQueueItemsByHelpdeskIDResponse response = new GetQueueItemsByHelpdeskIDResponse();

            try
            {
                var dataLayer = new QueueDataLayer();

                List<QueueItemDTO> queueItems = dataLayer.GetQueueItemsByHelpdeskID(id);

                if (queueItems.Count == 0)
                    throw new NotFoundException("No queue items found under helpdesk "+id);

                response.QueueItems = queueItems;
                response.Status = HttpStatusCode.OK;
            }
            catch (NotFoundException ex)
            {
                s_logger.Error(ex, "No queue items found!");
                response.Status = HttpStatusCode.NotFound;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.NotFound, "No queue items found!"));
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Unable to get queue items!");
                response.Status = HttpStatusCode.InternalServerError;
                response.StatusMessages.Add(new StatusMessage(HttpStatusCode.InternalServerError, "Unable to get queue items!"));
            }
            return response;
        }
    }
}
