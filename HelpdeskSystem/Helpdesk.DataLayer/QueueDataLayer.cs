using Helpdesk.Common.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Data.Models;
using Helpdesk.Common.Requests.Queue;

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
    }
}
