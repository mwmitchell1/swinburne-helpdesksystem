using Helpdesk.Common.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using Helpdesk.Data.Models;
using Helpdesk.Common.Requests.Queue;
using System.Linq;
using Helpdesk.Common.Extensions;

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
    }
}
