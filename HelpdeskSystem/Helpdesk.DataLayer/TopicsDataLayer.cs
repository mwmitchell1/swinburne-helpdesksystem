using System;
using System.Collections.Generic;
using System.Linq;
using Helpdesk.Common.DTOs;
using Helpdesk.Data.Models;
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
