using System;
using System.Collections.Generic;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Topics
{
    /// <summary>
    /// Used to indicate the result of getting all topics of a specific unit
    /// </summary>
    public class GetTopicsByUnitIDResponse : BaseResponse
    {
        public List<TopicDTO> Topics { get; set; }
    }
}
