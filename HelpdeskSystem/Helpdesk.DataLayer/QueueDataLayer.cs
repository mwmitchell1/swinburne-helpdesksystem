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
                    Description = request.Description,
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
                // E.g. you tried to provide TimeHelped when TimeHelped was alrady set for that queue item.
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
                item.Description = request.Description;
                context.SaveChanges();
                return true;
            }
        }

        /// <summary>
        /// This method retreives all queue items in a specific helpdesk from the database
        /// </summary>
        /// <param name="id">ID of the helpdesk to retrieve queue items from</param>
        /// <returns>A list of the queue items</returns>
        public List<QueueItemDTO> GetQueueItemsByHelpdeskID(int id)
        {
            List<QueueItemDTO> queueItemDTOs = new List<QueueItemDTO>();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unitIDs = context.Helpdeskunit.Include("Helpdeskunit").Where(hu => hu.HelpdeskId == id).Select(u => u.UnitId);
                var topicIDs = context.Topic.Where(t => unitIDs.Contains(t.UnitId)).Select(ti => ti.TopicId).ToList();
                var queueItems = context.Queueitem.Include("Topic.Unit").Include("Student").Where(qi => topicIDs.Contains(qi.TopicId)).ToList();

                foreach (Queueitem queueItem in queueItems)
                {
                    // Only get queueItems that haven't been removed yet.
                    if (queueItem.TimeRemoved == null)
                    {
                        QueueItemDTO queueItemDTO = DAO2DTO(queueItem);
                        var checkIn = context.Checkinqueueitem.Where(ch => ch.QueueItemId == queueItem.ItemId).FirstOrDefault();

                        if (checkIn != null)
                        {
                            queueItemDTO.CheckInId = checkIn.CheckInId;
                        }
                        queueItemDTOs.Add(queueItemDTO);
                    }
                }
            }
            return queueItemDTOs;
        }

        /// <summary>
        /// Used to retreive all of the queue items for a check in
        /// </summary>
        /// <param name="checkInId">The id of the students check in</param>
        /// <returns>The list of queue items for that check in</returns>
        public List<QueueItemDTO> GetQueueItemsByCheckIn(int checkInId)
        {
            List<QueueItemDTO> queueItems = new List<QueueItemDTO>();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var itemIds = context.Checkinqueueitem.Where(cq => cq.CheckInId == checkInId).Select(cq => cq.QueueItemId);

                foreach (int id in itemIds)
                {
                    var item = context.Queueitem.Include("Topic.Unit").Include("Student").Where(i => i.ItemId == id).FirstOrDefault();

                    if (item != null && item.TimeRemoved == null)
                    {
                        queueItems.Add(DAO2DTO(item));
                    }
                }
            }

            return queueItems;
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
                        cmd.CommandText = "GetAllQueueItems";
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
            queueItemDTO.Nickname = queueItem.Student.NickName;
            queueItemDTO.TopicId = queueItem.TopicId;
            queueItemDTO.Topic = queueItem.Topic.Name;
            queueItemDTO.Unit = queueItem.Topic.Unit.Name;
            queueItemDTO.TimeAdded = queueItem.TimeAdded;
            queueItemDTO.TimeHelped = queueItem.TimeHelped;
            queueItemDTO.TimeRemoved = queueItem.TimeRemoved;
            queueItemDTO.Description = queueItem.Description;

            return queueItemDTO;
        }

        /// <summary>
        /// Converts the queue item DTO to a DAO to interact with the database
        /// </summary>
        /// <param name="queueItemDTO">The DTO for the queue item</param>
        /// <returns>The DAO for the queue item</returns>
        private Queueitem DTO2DAO(QueueItemDTO queueItemDTO)
        {
            Queueitem queueItem = new Queueitem
            {
                ItemId = queueItemDTO.ItemId,
                StudentId = queueItemDTO.StudentId,
                TopicId = queueItemDTO.TopicId,
                Description = queueItemDTO.Description,
                TimeAdded = queueItemDTO.TimeAdded,
                TimeHelped = queueItemDTO.TimeHelped,
                TimeRemoved = queueItemDTO.TimeRemoved
            };

            return queueItem;
        }
    }
}
