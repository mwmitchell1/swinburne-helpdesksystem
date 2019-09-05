using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Helpdesk.Common.DTOs;
using Helpdesk.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog;

namespace Helpdesk.DataLayer
{
    public class TopicsDataLayer
    {
        private static Logger s_Logger = LogManager.GetCurrentClassLogger();

        public List<TopicDTO> GetTopicsByUnitID(int id)
        {
            List<TopicDTO> topicDTOs = new List<TopicDTO>();

            using (helpdesksystemContext context = new helpdesksystemContext())
            {
                var unitTopics = context.Topic.Where(ut => ut.UnitId == id).ToList();

                foreach (Topic unitTopic in unitTopics)
                {
                    topicDTOs.Add(DAO2DTO(unitTopic));
                }
            }
            return topicDTOs;
        }

        /// <summary>
        /// Used to get a datatable with all of the topic records
        /// </summary>
        /// <returns>Datatable with the topic records</returns>
        public DataTable GetTopicsAsDataTable()
        {
            DataTable topics = new DataTable();

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
                        cmd.CommandText = "getalltopics";
                        cmd.CommandType = CommandType.StoredProcedure;
                        using (var reader = cmd.ExecuteReader())
                        {
                            topics.Load(reader);
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

            return topics;
        }

        private TopicDTO DAO2DTO(Topic topic)
        {
            TopicDTO topicDTO = new TopicDTO
            {
                TopicId = topic.TopicId,
                UnitId = topic.UnitId,
                Name = topic.Name,
                IsDeleted = topic.IsDeleted
            };
            return topicDTO;
        }

        private Topic DTO2DAO(TopicDTO topicDTO)
        {
            Topic topic = new Topic
            {
                TopicId = topicDTO.TopicId,
                UnitId = topicDTO.UnitId,
                Name = topicDTO.Name,
                IsDeleted = topicDTO.IsDeleted
            };
            return topic;
        }
    }
}
