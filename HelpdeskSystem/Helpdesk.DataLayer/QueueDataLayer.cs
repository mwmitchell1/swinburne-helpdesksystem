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
using System.Data;
using System.Data.Common;

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

        /// <summary>
        /// Used to update a queue item status in the database.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool UpdateQueueItemStatus(int id, UpdateQueueItemStatusRequest request)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem item = context.Queueitem.FirstOrDefault(p => p.ItemId == id);

                if (item == null)
                    throw new NotFoundException($"Unable to find queue item with id [{id}]");

                if (request.TimeHelped != null && request.TimeRemoved == null && item.TimeHelped == null)
                {
                    // Update TimeHelped
                    item.TimeHelped = request.TimeHelped;
                    context.SaveChanges();
                    return true;
                }
                if (request.TimeRemoved != null && request.TimeHelped == null && item.TimeRemoved == null)
                {
                    if (request.TimeRemoved <= item.TimeHelped)
                    {
                        // TimeRemoved date is earlier than TimeHelped date - this can not happen.
                        return false;
                    }
                    // Update TimeRemoved
                    item.TimeRemoved = request.TimeRemoved;
                    context.SaveChanges();
                    return true;
                }
                // something went wrong. Shouldn't happen if the facade validation did its job.
                // Means both Helped and Removed values are assigned or null.
                // Could also mean a TimeHelped or TimeRemoved was provided twice.
                // E.g. you tried to provide TimeHelped when TimeHelped was alrady set in the table.
                return false;
            }
        }

        /// <summary>
        /// Edits the queue details in the database.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool UpdateQueueItem(int id, UpdateQueueItemRequest request)
        {
            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                Queueitem item = context.Queueitem.FirstOrDefault(p => p.ItemId == id);

                if (item == null)
                    throw new NotFoundException($"Unable to find queue item with id [{id}]");

                Topic topic = context.Topic.FirstOrDefault(t => t.TopicId == request.TopicID);

                // Check that the topic the queue item wants to update to actually exists.
                if (topic == null)
                    throw new NotFoundException($"Unable to find topic with id [{request.TopicID}]");

                item.TopicId = request.TopicID;
                context.SaveChanges();
                return true;
            }
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
                    if (queueItem != null && !queueItem.TimeRemoved.HasValue)
                    {
                        QueueItemDTO queueItemDTO = DAO2DTO(queueItem);
                        queueItemDTOs.Add(queueItemDTO);
                    }
                }
            }
            return queueItemDTOs;
        }

        /// <summary>
        /// Used to get a datatable with all of the helpdesk records
        /// </summary>
        /// <returns>Datatable with the helpdesk records</returns>
        public DataTable GetQueueItemsAsDataTable()
        {
            DataTable queueItems = new DataTable();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                DbConnection conn = context.Database.GetDbConnection();
                ConnectionState state = conn.State;

                try
                {
                    if (state != ConnectionState.Open)
                        conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "getallqueueitems";
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            queueItems.Load(reader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (state != ConnectionState.Closed)
                        conn.Close();
                }
            }

            return queueItems;
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
