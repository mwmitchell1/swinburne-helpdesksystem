using System;
using System.Collections.Generic;
using Helpdesk.Common.DTOs;

namespace Helpdesk.Common.Responses.Topics
{
    public class GetTopicsByUnitIDResponse : BaseResponse
    {
        public List<TopicDTO> Topics { get; set; }
    }
}
