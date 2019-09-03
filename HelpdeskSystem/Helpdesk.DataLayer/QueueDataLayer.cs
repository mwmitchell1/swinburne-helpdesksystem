using Helpdesk.Common.DTOs;
using Helpdesk.Common.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Data.Models;
using Helpdesk.Common.Requests.Queue;
using System.Linq;
using Helpdesk.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.DataLayer
{
    /// <summary>
    /// Used to handle any databases interactions for queues including CRUD, login and logout
    /// </summary>
    public class QueueDataLayer
    {
        /// <summary>
        /// Used to add a queue item to the database
        /// </summary>
        /// <param name="request">The request with the information to be added</param>
        /// <returns>The id of the queue item or null if not successful</returns>
        public int AddToQueue(AddToQueueRequest request)
        {
            int? id = null;

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem item = new Queueitem()
                {
                    StudentId = request.StudentID.Value,
                    TimeAdded = DateTime.Now,
                    TopicId = request.TopicID,
                };

                context.Queueitem.Add(item);
                context.SaveChanges();

                id = item.ItemId;

                if (request.CheckInID.HasValue)
                {
                    Checkinhistory checkinhistory = context.Checkinhistory.FirstOrDefault(ci => ci.CheckInId == request.CheckInID.Value);

                    if (checkinhistory == null)
                        throw new NotFoundException($"Unable to find queue item with id [{request.CheckInID.Value}]");

                    Checkinqueueitem checkinqueueitem = new Checkinqueueitem()
                    {
                        CheckInId = request.CheckInID.Value,
                        QueueItemId = item.ItemId,
                    };

                    context.Checkinqueueitem.Add(checkinqueueitem);
                }

                context.SaveChanges();
            }

            return id.Value;
        }

        public bool UpdateQueueItemStatus(UpdateQueueItemStatusRequest request)
        {
            bool result = false;

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem item = new Queueitem();

                if (request.TimeHelped != null && request.TimeRemoved == null)
                {
                    // Update TimeHelped
                    item.ItemId = request.ItemId;
                    item.TimeHelped = request.TimeHelped;
                    context.SaveChanges();
                    result = true;
                }
                else if (request.TimeRemoved != null && request.TimeHelped == null)
                {
                    // Update TimeRemoved
                    item.ItemId = request.ItemId;
                    item.TimeRemoved = request.TimeRemoved;
                    context.SaveChanges();
                    result = true;
                }
                else
                {
                    // something went wrong. Shouldn't happen if the facade validation did its job.
                    // Means both Helped and Removed values are assigned or null.
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// This method retrieves a list of all the queue items in the database
        /// </summary>
        /// <returns>A list of queue items retrieved from the database</returns>
        public List<QueueItemDTO> GetQueueItemsByHelpdeskID(int id)
        {
            List<QueueItemDTO> queueItemDTOs = new List<QueueItemDTO>();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unitIDs = context.Helpdeskunit.Include("Helpdeskunit").Where(hu => hu.HelpdeskId == id).Select(u => u.UnitId);
                var topicIDs = context.Topic.Where(t => unitIDs.Contains(t.UnitId)).Select(ti => ti.TopicId).ToList();
                var queueItems = context.Queueitem.Where(qi => topicIDs.Contains(qi.TopicId)).ToList();

                foreach (Queueitem queueItem in queueItems)
                {
                    if (queueItem != null)
                    {
                        QueueItemDTO queueItemDTO = DAO2DTO(queueItem);
                        queueItemDTOs.Add(queueItemDTO);
                    }
                }
            }
            return queueItemDTOs;
        }

        /// <summary>
        /// Converts the queue item DAO to a DTO to send to the front end
        /// </summary>
        /// <param name="queueItem">The DAO for the queue item</param>
        /// <returns>The DTO for the queue item</returns>
        private QueueItemDTO DAO2DTO(Queueitem queueItem)
        {
            QueueItemDTO queueItemDTO = null;

            queueItemDTO = new QueueItemDTO();
            queueItemDTO.ItemId = queueItem.ItemId;
            queueItemDTO.StudentId = queueItem.StudentId;
            queueItemDTO.TopicId = queueItem.TopicId;
            queueItemDTO.TimeAdded = queueItem.TimeAdded;
            queueItemDTO.TimeHelped = queueItem.TimeHelped;
            queueItemDTO.TimeRemoved = queueItem.TimeRemoved;

            return queueItemDTO;
        }

        /// <summary>
        /// Converts the queue item DTO to a DAO to interact with the database
        /// </summary>
        /// <param name="queueItemDTO">The DTO for the queue item</param>
        /// <returns>The DAO for the queue item</returns>
        private Queueitem DTO2DAO(QueueItemDTO queueItemDTO)
        {
            Queueitem queueItem = null;
            queueItem = new Queueitem();
            queueItem.ItemId = queueItemDTO.ItemId;
            queueItem.StudentId = queueItemDTO.StudentId;
            queueItem.TopicId = queueItemDTO.TopicId;
            queueItem.TimeAdded = queueItemDTO.TimeAdded;
            queueItem.TimeHelped = queueItemDTO.TimeHelped;
            queueItem.TimeRemoved = queueItemDTO.TimeRemoved;

            return queueItem;
        }
    }
}
